//
//  AlternativeInputUnity.c
//  Unity-iPhone
//
//  Created by Raimundas Sakalauskas on 15/09/15.
//
//

#import <objc/runtime.h>
#include <stdio.h>
#include <stdlib.h>
#include "AlternativeInputUnity.h"
#include "UnityView.h"


@implementation AlternativeInput

//load is called once per class
+ (void) load
{
    [self replaceMethods];
}

//replaces Touch methods in UnityView.mm, also adds an implementation of method traitCollectionDidChange
//(if user navigates to accessibility settings and changes ForceTouch settings the callback method in Unity will be called).
+ (void) replaceMethods
{
    SEL touchBegin = @selector(touchesBegan:withEvent:);
    Method orig = class_getInstanceMethod([UnityView class], touchBegin);
    Method replacement = class_getInstanceMethod([self class], touchBegin);
    method_exchangeImplementations(orig, replacement);
    
    SEL touchEnd = @selector(touchesEnded:withEvent:);
    Method orig1 = class_getInstanceMethod([UnityView class], touchEnd);
    Method replacement1 = class_getInstanceMethod([self class], touchEnd);
    method_exchangeImplementations(orig1, replacement1);
    
    SEL touchCancel = @selector(touchesCancelled:withEvent:);
    Method orig2 = class_getInstanceMethod([UnityView class], touchCancel);
    Method replacement2 = class_getInstanceMethod([self class], touchCancel);
    method_exchangeImplementations(orig2, replacement2);
    
    SEL touchMove = @selector(touchesMoved:withEvent:);
    Method orig3 = class_getInstanceMethod([UnityView class], touchMove);
    Method replacement3 = class_getInstanceMethod([self class], touchMove);
    method_exchangeImplementations(orig3, replacement3);
    

    if(NSClassFromString(@"UITraitCollection")) //only available in iOS8
        class_addMethod([UnityView class], @selector(traitCollectionDidChange:), (IMP)TraitCollectionDidChange, [[NSString stringWithFormat:@"@:%s", @encode(UITraitCollection)] UTF8String]);
}

//replacement methods
- (void)touchesBegan:(NSSet*)touches withEvent:(UIEvent*)event
{
    [[AlternativeInput instance] addTouches:touches];
    //even though this looks like recursion, this is the call to original method.
    [[AlternativeInput instance] touchesBegan:touches withEvent:event];
}
- (void)touchesEnded:(NSSet*)touches withEvent:(UIEvent*)event
{
    [[AlternativeInput instance] removeTouches:touches];
    [[AlternativeInput instance] touchesEnded:touches withEvent:event];
}
- (void)touchesCancelled:(NSSet*)touches withEvent:(UIEvent*)event
{
    [[AlternativeInput instance] removeTouches:touches];
    [[AlternativeInput instance] touchesCancelled:touches withEvent:event];
}
- (void)touchesMoved:(NSSet*)touches withEvent:(UIEvent*)event
{
    [[AlternativeInput instance] updateTouches:touches];
    [[AlternativeInput instance] touchesMoved:touches withEvent:event];
}

static void TraitCollectionDidChange(id self, SEL _cmd, UITraitCollection *previousTraitCollection)
{
    if ([[self traitCollection] respondsToSelector:@selector(forceTouchCapability)])
    {
        if (previousTraitCollection.forceTouchCapability != [self traitCollection].forceTouchCapability)
        {
			//if user added callback method
            if ([[AlternativeInput instance] callbackGameObject] && [[AlternativeInput instance] callbackMethod])
				UnitySendMessage(cStringCopy([[[AlternativeInput instance] callbackGameObject] UTF8String]), cStringCopy([[[AlternativeInput instance] callbackMethod] UTF8String]), cStringCopy([[NSString stringWithFormat:@"%d",[[AlternativeInput instance] getForceTouchState]] UTF8String]));
        }
    }
}
//end replacement methods


//lazy instantiate
+ (id) instance
{
    static AlternativeInput *instance = nil;
    @synchronized(self) {
        if (instance == nil)
            instance = [[self alloc] init];
    }
    return instance;
}

@synthesize touches = _touches;
@synthesize callbackMethod = _callbackMethod;
@synthesize callbackGameObject = _callbackGameObject;

//lazy instantiate
- (NSMutableDictionary *)touches
{
    if (!_touches) _touches = [[NSMutableDictionary alloc] init];
    return _touches;
}

- (void) addTouches:(NSSet*)touches
{
    for (UITouch *touch in touches)
    {
        int touchId = [self fingerIdForTouch:touch];
        if (touchId == -1)
        {
            [self.touches setObject:touch forKey:@([self newFingerId])];
            //NSLog(@"Adding touch. Count After: %lu", (unsigned long)[self.touches count]);
        }
    }
}

- (void) updateTouches:(NSSet*)touches
{
	[self addTouches:touches];
}

- (void) removeTouches:(NSSet*)touches
{
    for (UITouch *touch in touches)
    {
        int touchId = [self fingerIdForTouch:touch];
        //NSLog(@"Attepmting remove touch: %d. Count: %lu", touchId, (unsigned long)[self.touches count]);
        if (touchId != -1)
            [self.touches removeObjectForKey:@(touchId)];
        else
            NSLog(@"Touch Not Found!");
        //NSLog(@"Count: %lu", (unsigned long)[self.touches count]);
    }
}

//This is a bug with Unity. Sometimes when app is returning from the background it's possible to register the touch before
//UnityView becomes active. This touch has to be removed because it shifts touch ids.
- (NSMutableDictionary *) removeStaleTouch
{
    NSLog(@"Removing stale touch. Count with stale touch: %lu", (unsigned long)[self.touches count]);
    
    NSMutableDictionary *newTouches = [[NSMutableDictionary alloc] init];
    for(NSNumber *key in [self touches])
    {
        if ([key intValue] > 0)
        {
            int keyint = [key intValue];
            [newTouches setObject:[self.touches objectForKey:key] forKey:@(keyint  - 1)];
        }
    }
    [_touches removeAllObjects];
    _touches = newTouches;
    
    NSLog(@"Without stale touch: %lu", (unsigned long)[self.touches count]);    
    
    return [self touches];
}


- (int)getForceTouchState
{
    UIView *unityView = UnityGetGLView();
    if (NSClassFromString(@"UITraitCollection")) //Only available in iOS 8
    {
        if ([unityView.traitCollection respondsToSelector:@selector(forceTouchCapability)])
        {
            if (unityView.traitCollection.forceTouchCapability == UIForceTouchCapabilityAvailable)
                return 1;
            else if (unityView.traitCollection.forceTouchCapability == UIForceTouchCapabilityUnavailable)
                return 2;
            else if (unityView.traitCollection.forceTouchCapability == UIForceTouchCapabilityUnknown)
                return 3;
            else
                return 4;
        }
        else
        {
            return 4;
        }
    }
    else
    {
        return 4;
    }
}

- (void) setCallbackMethod:(NSString *)method forGameObject:(NSString *)gameObject
{
    //This is problematic.
    //String object received by calling [NSString stringWithUTF8String:] uses reference to const char* passed by managed code.
    //At the end of the method call from managed code, unity releases all the variables passed to native code.
    //See the problem here?:) Basically if we simply assing reference to strings passed as arguments, by the
    //time we need them they will be released (Only in 4.6, in 5.0+ it was fixed).
    //Objective-c compiler is optimized therefore [NSString stringWithString:] and [NSString copy] don't do deep
    //copy; resulting objects address same memory address. Therefore mutable copy is the only way to make deep copy
    //and retain the memory from being released by Unity engine.

    _callbackMethod = [method mutableCopy];
    _callbackGameObject = [gameObject mutableCopy];
}

- (void) removeCallbackMethod
{
    _callbackMethod = nil;
    _callbackGameObject = nil;
}


- (int) newFingerId
{
	int i = 0;
	while (YES)
	{
		if (![self.touches objectForKey:@(i)])
			return i;
		i++;
	}
	
//	  Legacy Code - replace method implementation with this code if you
//    need id algorithm used in previous versions of the plugin
//    NSString *temp = [NSString stringWithFormat:@"%i", arc4random_uniform(UINT16_MAX)];
//    while ([self.touches objectForKey:temp])
//    {
//        temp = [NSString stringWithFormat:@"%i", arc4random_uniform(UINT16_MAX)];
//    }
//    
//    return [temp intValue];
}

- (int) fingerIdForTouch:(UITouch *)touch
{
    for (NSNumber* key in [self.touches allKeys])
    {
        if ([self.touches objectForKey:key] == touch)
            return [key intValue];
    }
    return -1;
}
@end

//interface for communication with unity managed code
extern "C"
{
	unsigned const TOUCH_BINARY_SMALL_SIZE = 20;
	
	//@"Id"
	//@"Force"
	//@"MaxForce"
	//@"Radius"
	//@"RadiusTolerance"
	int getForceData(long *dataPtr, int touchCount)
	{
		NSMutableDictionary *touches = [[AlternativeInput instance] touches];

        if ([touches count] > touchCount)
        {
            //NSLog(@"Touch in array: %lu and in Unity: %d", [touches count], touchCount);
            touches = [[AlternativeInput instance] removeStaleTouch];
        }
        
		int dataLength = (int)[touches count] * TOUCH_BINARY_SMALL_SIZE;
		if (dataLength > 0)
		{
			unsigned char* data = (unsigned char*)malloc(dataLength);
			int touchIdx = 0;
			
			for (NSNumber* key in [touches allKeys])
			{
				UITouch *touch = [touches objectForKey:key];
				//determine force
				float force = 1.0;
				float maxForce = -1.0;
				if ([touch respondsToSelector:@selector(maximumPossibleForce)])
				{
					if (touch.maximumPossibleForce > 0)
					{
						force = touch.force;
						maxForce = touch.maximumPossibleForce;
					}
				}
				
				float radius = -1.0;
				float radiusTolerance = 0;
				if ([touch respondsToSelector:@selector(majorRadius)])
				{
					radius = touch.majorRadius;
					radiusTolerance = touch.majorRadiusTolerance;
				}
				
				WriteInt32(data + (touchIdx * TOUCH_BINARY_SMALL_SIZE), [key intValue]);
				WriteFloat(data + (touchIdx * TOUCH_BINARY_SMALL_SIZE) + 4, force);
				WriteFloat(data + (touchIdx * TOUCH_BINARY_SMALL_SIZE) + 8, maxForce);
				WriteFloat(data + (touchIdx * TOUCH_BINARY_SMALL_SIZE) + 12, radius);
				WriteFloat(data + (touchIdx * TOUCH_BINARY_SMALL_SIZE) + 16, radiusTolerance);
				
				touchIdx++;
			}
			
			*dataPtr = (long)data;
			return dataLength;
		}
		else
		{
			return 0;
		}
	}
	
	
	
    unsigned const TOUCH_BINARY_SIZE = 36;
	
    //@"Id"
    //@"X"
    //@"Y"
    //@"DeltaX"
    //@"DeltaY"
    //@"Force"
    //@"MaxForce"
    //@"Radius"
    //@"RadiusTolerance"
    int getNativeTouches(long *dataPtr, float unityScreenSize, int touchCount)
    {
        NSMutableDictionary *touches = [[AlternativeInput instance] touches];
        if ([touches count] > touchCount)
        {
            //NSLog(@"Touch in array: %lu and in Unity: %d", [touches count], touchCount);
            touches = [[AlternativeInput instance] removeStaleTouch];
        }
        
        CGRect bounds = [UnityGetGLView() bounds];
        float inputScale = getScaleFactor(unityScreenSize);
		
        int dataLength = (int)[touches count] * TOUCH_BINARY_SIZE;
        if (dataLength > 0)
        {
            unsigned char* data = (unsigned char*)malloc(dataLength);
            int touchIdx = 0;
            
            for (NSString* key in [touches allKeys])
            {
                UITouch *touch = [touches objectForKey:key];
                CGPoint pt = [touch locationInView:UnityGetGLView()];
                CGPoint lastpt =  [touch previousLocationInView:UnityGetGLView()];
                
                //determine force
                float force = 1.0;
                float maxForce = -1.0;
                if ([touch respondsToSelector:@selector(maximumPossibleForce)])
                {
                    if (touch.maximumPossibleForce > 0)
                    {
                        force = touch.force;
                        maxForce = touch.maximumPossibleForce;
                    }
                }
                
                float radius = -1.0;
                float radiusTolerance = 0;
                if ([touch respondsToSelector:@selector(majorRadius)])
                {
                    radius = touch.majorRadius;
                    radiusTolerance = touch.majorRadiusTolerance;
                }
                
                
                WriteInt32(data + (touchIdx * TOUCH_BINARY_SIZE), [key intValue]);
                WriteFloat(data + (touchIdx * TOUCH_BINARY_SIZE) + 4, pt.x * inputScale);
                WriteFloat(data + (touchIdx * TOUCH_BINARY_SIZE) + 8, (bounds.size.height - pt.y) * inputScale);
                WriteFloat(data + (touchIdx * TOUCH_BINARY_SIZE) + 12, (pt.x - lastpt.x) * inputScale);
                WriteFloat(data + (touchIdx * TOUCH_BINARY_SIZE) + 16, (lastpt.y - pt.y) * inputScale);
                WriteFloat(data + (touchIdx * TOUCH_BINARY_SIZE) + 20, force);
                WriteFloat(data + (touchIdx * TOUCH_BINARY_SIZE) + 24, maxForce);
                WriteFloat(data + (touchIdx * TOUCH_BINARY_SIZE) + 28, radius);
                WriteFloat(data + (touchIdx * TOUCH_BINARY_SIZE) + 32, radiusTolerance);
                
                touchIdx++;
            }
            
            *dataPtr = (long)data;
            return dataLength;
        }
        else
        {
            return 0;
        }
    }
    
    float getScaleFactor(float unityScreenSize)
    {
        CGRect bounds = [UnityGetGLView() bounds];
        float iOSSize = ((bounds.size.height > bounds.size.width) ? bounds.size.height : bounds.size.width);
        return unityScreenSize / iOSSize;
    }
	
    
    void setCallbackMethod(const char* gameObject, const char* methodName)
    {
        [[AlternativeInput instance] setCallbackMethod:CreateNSString(methodName) forGameObject:CreateNSString(gameObject)];
    }
    
    void removeCallbackMethod()
    {
        [[AlternativeInput instance] removeCallbackMethod];
    }
	
	
	int getForceTouchState()
	{
		return [[AlternativeInput instance] getForceTouchState];
	}
	
    bool supportsTouchRadius()
    {
        return [[UITouch class] instancesRespondToSelector:@selector(majorRadius)];
    }
}





//helper methods
void	WriteInt32(unsigned char* data, int32_t value)
{
    char* ref = (char*) &value;
    memcpy(data, ref, sizeof(int32_t));
}

void	WriteInt16(unsigned char* data, int16_t value)
{
    char* ref = (char*) &value;
    memcpy(data, ref, sizeof(int16_t));
}

void	WriteFloat(unsigned char* data, float_t value)
{
    char* ref = (char*) &value;
    memcpy(data, ref, sizeof(float_t));
}

char* cStringCopy(const char* string)
{
    if (string == NULL)
        return NULL;
    
    char* res = (char*)malloc(strlen(string) + 1);
    strcpy(res, string);
    
    return res;
}

NSString* CreateNSString(const char* string)
{
    if (string != NULL)
        return [NSString stringWithUTF8String:string];
    else
        return [NSString stringWithUTF8String:""];
}