using UnityEngine;
using System.Collections;
using UnityEditor.Callbacks;
using UnityEditor;
using UnityEditor.iOS.Xcode;
using System;
using System.IO;
using System.Linq;
//using Mk.Common;
using System.Collections.Generic;

public class Postbuild {
	private const string NSPhotoLibraryUsageDescription = "NSPhotoLibraryUsageDescription";

	[PostProcessBuildAttribute (0)]

	public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
	{
		LinkLibraries(target, pathToBuiltProject);
		AddedPlistDocument (target, pathToBuiltProject);
	}

	public static void AddedPlistDocument(BuildTarget target, string pathToBuiltProject) {
		Debug.Log("GveBuild.OnPostprocessBuild " + target + " " + pathToBuiltProject);
		switch (target) {
		case BuildTarget.iOS:
			string plistFilePath = pathToBuiltProject + Path.DirectorySeparatorChar + "Info.plist";
			// 10/5/16 The code that uses Xcode is flagged as missing in editor, but it compiles when we return to Unity and works
			// when build is run.
			// Q: why doesn't this work? A: 2nd link says it is fixed in 5.4.2, we'll see
			// https://issuetracker.unity3d.com/issues/ios-monodevelop-unityeditor-dot-ios-dot-xcode-namespace-isnt-recognised-in-monodevelop
			// https://issuetracker.unity3d.com/issues/unity-does-not-include-unityeditor-dot-ios-dot-xcode-in-project-file

			// Read the existing plist file
			try {
				PlistDocument plist = new PlistDocument();
				plist.ReadFromFile(plistFilePath);
				Debug.Log("GveBuild.OnPostprocessBuild successfully read " + plistFilePath + ": " + (plist.root).ToString());

				// Add our modifications
				if (plist.root.values.ContainsKey(NSPhotoLibraryUsageDescription)) {
					Debug.LogError("GveBuild.OnPostprocessBuild key already set? Do not overwrite: " + NSPhotoLibraryUsageDescription + " = " + plist.root.values[NSPhotoLibraryUsageDescription].ToString());
				} else {
					plist.root.values[NSPhotoLibraryUsageDescription] = new PlistElementString("Photo Libraries uesd for listing albunm.");
					Debug.Log("GveBuild.OnPostprocessBuild added NSCameraUsageDescription = " + (plist.root.values[NSPhotoLibraryUsageDescription]).ToString());
				}

				// Write the modified file
				string plistPathNew = plistFilePath + ".new";
				plist.WriteToFile(plistPathNew);

				// Replace the original file. Note subsequent build steps modify the plist file further (e.g. currently facebook entries are added after this script runs)
				if (true) {
					// Delete the old file
					File.Delete(plistFilePath);
				} else {
					// Keep the old file for diff while testing
					string plistPathOld = plistFilePath + ".orig";
					File.Move(plistFilePath, plistPathOld);
				}
				File.Move(plistPathNew, plistFilePath);
				Debug.Log("GveBuild.OnPostprocessBuild successfully updated " + plistFilePath);
			} catch (Exception e) {
				Debug.LogError("GveBuild.OnPostprocessBuild PList error " + e);
			}
			break;

		default:
			// nada
			break;
		}
	}


	public static void LinkLibraries(BuildTarget target, string pathToBuiltProject)
	{
		if(target == BuildTarget.iOS)
		{
			var projectPath = pathToBuiltProject + "/Unity-iPhone.xcodeproj/project.pbxproj";
			PBXProject pbxProject = new PBXProject ();
			pbxProject.ReadFromFile (projectPath);
			string targetGuid = pbxProject.TargetGuidByName ("Unity-iPhone");

			pbxProject.AddFrameworkToProject(targetGuid, "AssetsLibrary.framework", false);
			File.WriteAllText (projectPath, pbxProject.WriteToString ());
//			if (project == null)
//				project = new PBXProject ();
//			string TargetGuid = project.TargetGuidByName ("Unity-iPhone");
//
//			project.AddFrameworkToProject (TargetGuid, "AssetsLibrary.framework", false);
////			
//			string contents = File.ReadAllText(projectFile);
//
//			contents = "/* Changed By Editor */" + contents;
//
//			contents = contents.Replace(
//				"586D4D0D87A9D59FAB9A474E /* ARKit.framework in Frameworks */ = {isa = PBXBuildFile; fileRef = 5AD74739B6358491DC81FD3A /* ARKit.framework */; };",
//				"586D4D0D87A9D59FAB9A474E /* ARKit.framework in Frameworks */ = {isa = PBXBuildFile; fileRef = 5AD74739B6358491DC81FD3A /* ARKit.framework */; };\n\t\t" +
//				"E36F7B961F9101500035406B /* AssetsLibrary.framework in Frameworks */ = {isa = PBXBuildFile; fileRef = E36F7B951F9101500035406B /* AssetsLibrary.framework */; };");
//			
//			contents = contents.Replace(
//				"5AD74739B6358491DC81FD3A /* ARKit.framework */ = {isa = PBXFileReference; lastKnownFileType = wrapper.framework; name = ARKit.framework; path = System/Library/Frameworks/ARKit.framework; sourceTree = SDKROOT; };",
//				"5AD74739B6358491DC81FD3A /* ARKit.framework */ = {isa = PBXFileReference; lastKnownFileType = wrapper.framework; name = ARKit.framework; path = System/Library/Frameworks/ARKit.framework; sourceTree = SDKROOT; };\n\t\t" +
//				"E36F7B951F9101500035406B /* AssetsLibrary.framework */ = {isa = PBXFileReference; lastKnownFileType = wrapper.framework; name = AssetsLibrary.framework; path = System/Library/Frameworks/AssetsLibrary.framework; sourceTree = SDKROOT; };");
//			
//			contents = contents.Replace(
//				"586D4D0D87A9D59FAB9A474E /* ARKit.framework in Frameworks */,",
//				"586D4D0D87A9D59FAB9A474E /* ARKit.framework in Frameworks */,\n\t\t\t\t" +
//				"E36F7B961F9101500035406B /* AssetsLibrary.framework in Frameworks */");
//			
//
//			contents = contents.Replace(
//				"5AD74739B6358491DC81FD3A /* ARKit.framework */,",
//				"5AD74739B6358491DC81FD3A /* ARKit.framework */,\n\t\t\t\t" +
//				"E36F7B951F9101500035406B /* AssetsLibrary.framework,");
//			
//			File.WriteAllText(projectFile, contents);
		}
	}


	void Start() {
	}
}
