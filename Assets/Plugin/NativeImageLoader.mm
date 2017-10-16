#import "NativeImageLoader.h"


static int count=0;

static getPhotoLibViewController *_instance = [getPhotoLibViewController sharedInstance];

@implementation getPhotoLibViewController

+(getPhotoLibViewController *) sharedInstance {
    if(!_instance) {
        _instance = [[getPhotoLibViewController alloc] init];
    }
    return _instance;
}
+(NSArray *)GetImages
{
    return [_instance getImages];
}

+(void)GetAllPictures
{
    [_instance getAllPictures];
}

-(NSArray *)getImages
{
    return imageArray;
}

-(void)getAllPictures
{
    imageArray=[[NSArray alloc] init];
    mutableArray =[[NSMutableArray alloc]init];
    NSMutableArray* assetURLDictionaries = [[NSMutableArray alloc] init];

    library = [[ALAssetsLibrary alloc] init];
    
    void (^assetEnumerator)( ALAsset *, NSUInteger, BOOL *) = ^(ALAsset *result, NSUInteger index, BOOL *stop) {
        if(result != nil) {
            if([[result valueForProperty:ALAssetPropertyType] isEqualToString:ALAssetTypePhoto]) {
                [assetURLDictionaries addObject:[result valueForProperty:ALAssetPropertyURLs]];
                NSURL *url= (NSURL*) [[result defaultRepresentation]url]; 
                // NSString *path = [url absoluteString];

                // UnitySendMessage("_Scripts", "ImagesLoad", [path cStringUsingEncoding:NSUTF8StringEncoding]);

                // NSLog(@"URLS :: %@", path);
                // NSString *path = [url ]

                // NSLog(@"%@ %@ URLS %@", count, [mutableArray count], url);
                // [mutableArray addObject:url];

                // if([mutableArray count] == count) {
                //     imageArray=[[NSArray alloc] initWithArray:mutableArray];
                //     [self allPhotosCollected:imageArray];
                // }


                [library assetForURL:url 
                    resultBlock:^(ALAsset *asset) {
                        [mutableArray addObject:[UIImage imageWithCGImage:[[asset defaultRepresentation] fullScreenImage]]];
                        
                        if ([mutableArray count]==count)
                        {
                            imageArray=[[NSArray alloc] initWithArray:mutableArray];
                            [self allPhotosCollected:imageArray];
                        }
                    }
                    
                    failureBlock:^(NSError *error) { 
                        NSLog(@"operation was not successfull!"); 
                    }
                ]; 
            } 
        }
    };
    NSMutableArray *assetGroups = [[NSMutableArray alloc] init];
    
    void (^ assetGroupEnumerator) ( ALAssetsGroup *, BOOL *)= ^(ALAssetsGroup *group, BOOL *stop) {
        if(group != nil) {
            [group enumerateAssetsUsingBlock:assetEnumerator];
            [assetGroups addObject:group];
            count=[group numberOfAssets];
        }
    };
    assetGroups = [[NSMutableArray alloc] init];
    
    [library 
        enumerateGroupsWithTypes:ALAssetsGroupAll
        usingBlock:assetGroupEnumerator
        failureBlock:^(NSError *error) {NSLog(@"There is an error");}
    ];
}

-(void)allPhotosCollected:(NSArray*)imgArray
{
    //write your code here after getting all the photos from library...
    NSLog(@"all pictures are %@",imgArray);
    NSString* istr = [NSString stringWithFormat:@"%i",[imgArray count]];
    // for(UIImage *img in imgArray) {
    //     NSLog(@"EACH PICTURE : %@", img);
    //     NSData *pictureData = UIImageJPEGRepresentation(img, 1.0);
    //     NSString *pictureString = [[NSString alloc] initWithData:pictureData encoding:NSUTF8StringEncoding];
    //     NSLog(@"EACH PICTURE : %@", pictureString);
    // }
    UnitySendMessage("_Scripts", "ImagesLoad", [istr cStringUsingEncoding:NSUTF8StringEncoding]);
}

@end

extern "C" 
{
    void _RequestGalleryImage() {
        printf("RequestGalleryImage\n");
        [getPhotoLibViewController GetAllPictures];
    }

    void _GetImageBytes(int idx, intptr_t* ptr)
    {
        UIImage *img = [[getPhotoLibViewController GetImages] objectAtIndex:idx];
        // NSString *byteArray = [UIImageJPEGRepresentation(img, 1.0) base64EncodedStringWithOptions:NSDataBase64Encoding64CharacterLineLength];
        
         char * bytes = (char *)malloc(img.size.width * img.size.height * 4);

        // for(int w = 0; w < img.size.width; w++) {
        //     for(int h = 0; h < img.size.height; h++) {
        //         int width = img.size.width;
        //         (bytes)[(w + h * width) * 4 + 0] = 255;
        //         (bytes)[(w + h * width) * 4 + 1] = 0;
        //         (bytes)[(w + h * width) * 4 + 2] = 0;
        //         (bytes)[(w + h * width) * 4 + 3] = 255;
        //     }
        // }
        CGColorSpaceRef colorSpace = CGColorSpaceCreateDeviceRGB();
        CGImageRef imageRef = [img CGImage];
        
        CGContextRef bitmap = CGBitmapContextCreate(
                                                    bytes,
                                                    img.size.width,
                                                    img.size.height,
                                                    8,
                                                    img.size.width * 4,
                                                    colorSpace,
                                                    kCGBitmapByteOrder32Little | kCGImageAlphaPremultipliedFirst);
        CGContextDrawImage(bitmap, CGRectMake(0, 0, img.size.width, img.size.height), imageRef);
        CGContextRelease(bitmap);
        CGColorSpaceRelease(colorSpace);
        (*ptr) = (intptr_t)bytes;
    }
    
    void _GetImageSize(int idx, int *width, int *height) 
    {
        UIImage *img = [[getPhotoLibViewController GetImages] objectAtIndex:idx];
        (*width) = img.size.width;
        (*height) = img.size.height;
    }
}
