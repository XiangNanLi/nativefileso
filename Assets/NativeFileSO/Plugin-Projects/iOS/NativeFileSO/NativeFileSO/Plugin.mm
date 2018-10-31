//
//  Plugin.m
//  NativeFileSO
//
//  Created by Keiwan Donyagard on 14.10.18.
//  Copyright © 2018 Keiwan Donyagard. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "NativeFileSO.h"
#import "NativeFileOpenURLBuffer.h"
#import "UnityCallbackFunction.h"

extern "C" {
    
    void pluginSetCallback(UnityCallbackFunction callback) {
        [[NativeFileOpenURLBuffer instance] setCallback:callback];
    }
    
    int pluginGetNumberOfOpenedFiles() {
        return [[NativeFileOpenURLBuffer instance] getNumberOfOpenedFiles];
    }
    
    const char* pluginGetFilenameForFileAtIndex(int i) {
        NativeFileSOOpenedFile *file = [[NativeFileOpenURLBuffer instance] getOpenedFileAtIndex:i];
        return [file.filename UTF8String];
    }
    
    unsigned long pluginGetDataLengthForFileAtIndex(int i) {
        NativeFileSOOpenedFile *file = [[NativeFileOpenURLBuffer instance] getOpenedFileAtIndex:i];
        return file.data.length;
    }
    
    const void *pluginGetDataForFileAtIndex(int i) {
        NativeFileSOOpenedFile *file = [[NativeFileOpenURLBuffer instance] getOpenedFileAtIndex:i];
        return file.data.bytes;
    }
    
    void pluginResetLoadedFile() {
        [[NativeFileOpenURLBuffer instance] reset];
    }
    
    
    void pluginOpenFile(const char* utis, bool canSelectMultiple) {
        
        [NativeFileSO fileOpen:[NSString stringWithUTF8String:utis]
                allowsMultiple:canSelectMultiple];
    }
    
    void pluginSaveFile(const char* srcPath,
                   const char* name) {
        
        [NativeFileSO fileSave:[NSString stringWithUTF8String:srcPath]
                          name:[NSString stringWithUTF8String:name]];
    }
}