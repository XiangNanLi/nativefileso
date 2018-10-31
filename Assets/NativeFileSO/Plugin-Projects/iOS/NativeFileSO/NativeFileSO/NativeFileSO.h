//
//  NativeFileSO.h
//  NativeFileSO
//
//  Created by Keiwan Donyagard on 14.10.18.
//  Copyright © 2018 Keiwan Donyagard. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "NativeFileOpenURLBuffer.h"
#import "UIKit/UIKit.h"

@interface NativeFileSO : NSObject

+ (void) fileOpen:(NSString *)UTIs
   allowsMultiple:(bool)allowsMultiple;

+ (void) fileSave:(NSString *)srcPath
                //extension:(NSString *)extension
             name:(NSString *)name;

@end