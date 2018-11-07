﻿using System;
using Keiwando.NativeFileSO;

// TODO: Don't include in package
namespace Keiwando.NativeFileSO.Samples { 

	public class CustomFileTypes {

		public static readonly SupportedFileType evol = new SupportedFileType {
			Name = "Evolution Save File",
			Extension = "evol",
			Owner = true,
			AppleUTI = "com.keiwando.Evolution.evol",
			AppleConformsToUTI = "public.plain-text",
			MimeType = "text/plain"
		};

		public static readonly SupportedFileType creat = new SupportedFileType {
			Name = "Evolution Creature Save File",
			Extension = "creat",
			Owner = true,
			AppleUTI = "com.keiwando.Evolution.creat",
			AppleConformsToUTI = "public.plain-text",
			MimeType = "text/plain"
		};
	}
}