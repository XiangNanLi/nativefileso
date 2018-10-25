﻿#if UNITY_STANDALONE_WIN

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Ookii.Dialogs;
using UnityEngine;

namespace Keiwando.NativeFileSO {

	public class NativeFileSOWindows: INativeFileSODesktop {

		private class Win32Window: IWin32Window {
			public Win32Window(IntPtr ptr) {
				Handle = ptr;
			}
			public IntPtr Handle { get; set; }
		}

		[DllImport("user32.dll")]
		private static extern IntPtr GetActiveWindow();

		public static NativeFileSOWindows shared = new NativeFileSOWindows();

		private static OpenedFile[] _noFiles = new OpenedFile[0];
		private static string[] _noPaths = new string[0];

		private bool isBusy = false;

		public void OpenFile(SupportedFileType[] fileTypes, Action<bool, OpenedFile> onCompletion) { 

			var openedFiles = OpenFilesSync(fileTypes, false);
		
			if (onCompletion != null) {
				if (openedFiles.Length > 0) {
					onCompletion(true, openedFiles[0]);
				} else {
					onCompletion(false, null);
				}
			}
		}

		public void OpenFiles(SupportedFileType[] supportedTypes, Action<bool, OpenedFile[]> onCompletion) {
			OpenFiles(supportedTypes, true, "", "", onCompletion);
		}

		public void SaveFile(FileToSave file) { 
			
			SaveFile(file, "", "");
		}

		// MARK: - INativeFileSODesktop

		public void OpenFiles(SupportedFileType[] fileTypes, bool canSelectMultiple,
					   		  string title, string directory, 
					   		  Action<bool, OpenedFile[]> onCompletion) {
			
			var openedFiles = OpenFilesSync(fileTypes);
			isBusy = false;
			var filesWereOpened = openedFiles.Length != 0;
			if (onCompletion != null) {
				onCompletion(filesWereOpened, openedFiles);
			}
		}

		public OpenedFile[] OpenFilesSync(SupportedFileType[] fileTypes, bool canSelectMultiple = true, 
								   		  string title = "", string directory = "") {			

			var paths = SelectOpenPathsSync(fileTypes, canSelectMultiple, title, directory);
			var openedFiles = new List<OpenedFile>();
			
			foreach (var path in paths) {
				var file = NativeFileSOMacWin.FileFromPath(path);
				if (file != null) {
					openedFiles.Add(file);
				}
			}
			isBusy = false;

			return openedFiles.ToArray();
		}

		public void SelectOpenPaths(SupportedFileType[] fileTypes, bool canSelectMultiple,
							 		string title, string directory,
							 		Action<bool, string[]> onCompletion) {
			
			var paths = SelectOpenPathsSync(fileTypes, canSelectMultiple, title, directory);
			isBusy = false;
			if (onCompletion != null) {
				var pathsSelected = paths.Length != 0;
				onCompletion(pathsSelected, paths);
			} 
		}

		public string[] SelectOpenPathsSync(SupportedFileType[] fileTypes, bool canSelectMultiple = true,
									 		string title = "", string directory = "") {

			if (isBusy) { return _noPaths; }
			isBusy = true;

			var dialog = new VistaOpenFileDialog();
			
			dialog.Multiselect = canSelectMultiple;
			dialog.Title = title;

			if (string.IsNullOrEmpty(directory)) {
				dialog.RestoreDirectory = true;
			} else {
				dialog.FileName = EnsureStringEndsWith(directory, Path.DirectorySeparatorChar.ToString());
			}
			
			if (fileTypes != null && fileTypes.Length > 0) {
				dialog.Filter = EncodeFilters(fileTypes);
			}

			var result = dialog.ShowDialog(new Win32Window(GetActiveWindow()));
			isBusy = false;

			return result == DialogResult.OK ? dialog.FileNames : _noPaths;
		}

		public void SaveFile(FileToSave file, string title, string directory) {

			if (isBusy) return;
			isBusy = true;

			var dialog = new VistaSaveFileDialog();
			
			if (string.IsNullOrEmpty(directory)) {
				dialog.RestoreDirectory = true;
			} else {
				dialog.FileName = CreateFilenameForSaveDialog(directory, file.Name);
			}

			dialog.DefaultExt = file.Extension;
			if (dialog.DefaultExt.Length > 0) {
				dialog.AddExtension = true;
				dialog.SupportMultiDottedExtensions = true;
			}
			if (file.FileType != null) {
				dialog.Filter = EncodeFilters(new []{ file.FileType });
			}

			dialog.Title = title;
			dialog.InitialDirectory = directory;
			dialog.RestoreDirectory = true;

			var result = dialog.ShowDialog(new Win32Window(GetActiveWindow()));
			if (result == DialogResult.OK) {
				NativeFileSOMacWin.SaveFileToPath(file, dialog.FileName);
			}
			dialog.Dispose();
			isBusy = false;
		}

		// MARK: - Private Helpers

		private string CreateFilenameForSaveDialog(string directory, string name) {

			var sep = Path.DirectorySeparatorChar.ToString();
			if (!directory.EndsWith(sep)) {
				return string.Format("{0}{1}{2}", directory, sep, name);
			} else {
				return string.Format("{0}{1}", directory, name);
			}
		}

		private string EnsureStringEndsWith(string self, string end) {
			if(!self.EndsWith(end)) {
				return self + end;
			} else {
				return self;
			}
		}

		private string EncodeFilters(SupportedFileType[] types) {
			return string.Join("|", types.Select(delegate(SupportedFileType x){
				var ext = x.Extension.Equals(string.Empty) ? "*" : x.Extension;
				return string.Format("{0} (*.{1})|*.{1}", x.Name, ext);
			}).ToArray());

		}
	}
}

#endif