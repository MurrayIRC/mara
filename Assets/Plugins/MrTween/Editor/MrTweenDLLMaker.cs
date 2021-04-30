// enable .NET 2.0 (NOT Subset) and uncomment the following line to use the DLL builder
//#define DOT_NET_SUBSET_IS_NOT_SET_IN_API_COMPATIBILITY_MODE

#if UNITY_EDITOR && DOT_NET_SUBSET_IS_NOT_SET_IN_API_COMPATIBILITY_MODE 
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.IO;

public class MrTweenDLLMaker : Editor
{
	[MenuItem( "File/Create Mr. Tween DLLs..." )]
	static void CreateDLL()
	{
		if( CreateDLL( false, "MrTween.dll" ) && createDLL( true, "MrTween.Editor.dll" ) )
			EditorUtility.DisplayDialog( "MrTween", "MrTween DLLs should now be on your desktop.", "OK" );
	}
		
	static bool CreateDLL( bool isEditorDLL, string DLLName )
	{
		var compileParams = new CompilerParameters();
		compileParams.OutputAssembly = Path.Combine( System.Environment.GetFolderPath( System.Environment.SpecialFolder.Desktop ), DLLName );
		compileParams.CompilerOptions = "/optimize";

		if( isEditorDLL )
			compileParams.CompilerOptions += " /define:UNITY_EDITOR";
		
		compileParams.ReferencedAssemblies.Add( Path.Combine( EditorApplication.applicationContentsPath, "Frameworks/Managed/UnityEngine.dll" ) );
		compileParams.ReferencedAssemblies.Add( Path.Combine( EditorApplication.applicationContentsPath, "UnityExtensions/Unity/GUISystem/UnityEngine.UI.dll" ) );

		if( isEditorDLL )
		{
			compileParams.ReferencedAssemblies.Add( Path.Combine( EditorApplication.applicationContentsPath, "Frameworks/Managed/UnityEditor.dll" ) );
			compileParams.ReferencedAssemblies.Add( Path.Combine( System.Environment.GetFolderPath( System.Environment.SpecialFolder.Desktop ), "MrTween.dll" ) );
		}

		var source = isEditorDLL ? getSourceForEditorDLL() : getSourceForDLL();

		var codeProvider = new CSharpCodeProvider( new Dictionary<string,string> { { "CompilerVersion", "v3.0" } } );
		var compilerResults = codeProvider.CompileAssemblyFromSource( compileParams, source );

		if( compilerResults.Errors.Count > 0 )
		{
			Debug.Log( "Errors creating DLL: " + DLLName );
			foreach( var error in compilerResults.Errors )
				Debug.LogError( error.ToString() );

			return false;
		}

		return true;
	}

	static string[] GetSourceForDLL( string path = "Assets/Plugins/MrTween/" )
	{
		var source = new List<string>();

		foreach( var file in Directory.GetFiles( path, "*.cs", SearchOption.AllDirectories ) )
		{
			if( !file.Contains( "DummySpline" ) && !file.Contains( "Demo" ) && !file.Contains( "/Editor" ) )
				source.Add( File.ReadAllText( file ) );
		}

		return source.ToArray();
	}


	static string[] GetSourceForEditorDLL( string path = "Assets/Plugins/MrTween/" )
	{
		var source = new List<string>();

		foreach( var file in Directory.GetFiles( path, "DummySpline*.cs", SearchOption.AllDirectories ) )
		{
			source.Add( File.ReadAllText( file ) );
		}

		return source.ToArray();
	}

}
#endif
