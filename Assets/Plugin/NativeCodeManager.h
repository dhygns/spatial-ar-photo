#import <Foundation/Foundation.h>

extern UIViewController *UnityGetGLViewController();

@interface NativeCodeManager : UIViewController<UINavigationControllerDelegate, UIImagePickerControllerDelegate>
{
    UIImagePickerController * imagePicker;
}

@property (retain)UIImagePickerController *imagePicker;

+ (NativeCodeManager *) sharedInstance;
- (id)init;
- (void)dealloc;
- (void)imagePickerController:(UIImagePickerController *)picker didFinishPickingImageWithInfo:(NSDictionary<NSString *,id> *) info;
- (void)imagePickerControllerDidCancel:(UIImagePickerController *)picker;

@end