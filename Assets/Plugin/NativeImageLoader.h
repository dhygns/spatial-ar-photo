#import <Foundation/Foundation.h>
// #import <UIKit/UIKit.h>
#include <AssetsLibrary/AssetsLibrary.h>


@interface getPhotoLibViewController : UIViewController
{
    ALAssetsLibrary *library;
    NSArray *imageArray;
    NSMutableArray *mutableArray;
}

+ (getPhotoLibViewController *) sharedInstance;
+ (NSArray *)GetImages;
+ (void)GetAllPicture;

- (NSArray *)getImages;
- (void)getAllPictures;
- (void)allPhotosCollected:(NSArray*)imgArray;

 @end
