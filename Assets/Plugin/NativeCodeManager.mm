#import "NativeCodeManager.h"

static NativeCodeManager *_instance = [NativeCodeManager sharedInstance];
@implementation NativeCodeManager;

@synthesize imagePicker;

+ (NativeCodeManager *) sharedInstance;
{
    return _instance;
}

+ (void) initialize
{
    if(!_instance) {
        _instance = [[NativeCodeManager alloc] init];
    }
}

- (id) init
{
    self = [super init];
    if(!self) return nil;

    imagePicker = [[UIImagePickerController alloc]init];
    imagePicker.delegate = self;
    imagePicker.allowsEditing = YES;

    return self;
}

- (void) dealloc
{
    // [imagePicker release];
    // imagePicker = nil;
    // [super dealloc];
}

- (void) imagePickerController:(UIImagePickerController *)picker didFinishPickingMediaWithInfo:(NSDictionary<NSString *,id> *) info {
    printf("**\n");
    
    UIImage *img = [info objectForKey: UIImagePickerControllerEditedImage];
    if(!img) img = [info objectForKey: UIImagePickerControllerOriginalImage];
    printf("0\n");
    UIGraphicsBeginImageContextWithOptions (CGSizeMake(300, 300), NO, 0.0);
    [img drawInRect:CGRectMake(0, 0, 300, 300)];
    UIImage *newImage = UIGraphicsGetImageFromCurrentImageContext();
    UIGraphicsEndImageContext();

    printf("1\n");
    NSData* pickedData = UIImageJPEGRepresentation(newImage, 0.6);
    NSLog(@"length : %lu", (unsigned long)[pickedData length]);

    printf("2\n");    
    NSArray *paths = NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES);
    NSString *path = [[paths objectAtIndex:0] stringByAppendingPathComponent:@"temp.jpg"];
    [pickedData writeToFile:path atomically:YES];

    printf("%s", [path cStringUsingEncoding:NSUTF8StringEncoding]);
    UnitySendMessage("_Scripts", "UpdateImage", [path cStringUsingEncoding:NSUTF8StringEncoding]);

    [UnityGetGLViewController() dismissViewControllerAnimated:YES completion:NULL];

}

- (void)imagePickerControllerDidCancel:(UIImagePickerController *) picker {
    [UnityGetGLViewController() dismissViewControllerAnimated:YES completion:NULL];
}
@end

extern "C" 
{
    void RequestCameraImage() {
        printf("RequestCameraImage\n");
        
        if([UIImagePickerController isSourceTypeAvailable:UIImagePickerControllerSourceTypeCamera])
        {
            printf("isSource here1\n");
            [NativeCodeManager sharedInstance] .imagePicker.sourceType = UIImagePickerControllerSourceTypePhotoLibrary;
            printf("isSource here2\n");            
            [UnityGetGLViewController() presentViewController:[NativeCodeManager sharedInstance].imagePicker animated:YES completion:NULL];
        } else {
            printf("isSource NOT here");

        }
    }
}