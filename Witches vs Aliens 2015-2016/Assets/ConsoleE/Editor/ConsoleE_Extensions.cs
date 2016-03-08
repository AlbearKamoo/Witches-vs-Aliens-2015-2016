/*
 * This file allows you to:
 * 
 *		1) Edit the right-click context menus for ConsoleE
 *		2) Intercept or override the event when opening files or selecting items
 *		3) Format display string for extra info such as Time.time
 *	
 *		ConsoleE_Extensions.cs should be placed inside a folder with the name "Editor".
 * 
 *		OnFormatTime and OnFormatFrame are availabing in ConsoleE free.
 *		The remaining hooks require a pro license of ConsoleE.
 */

#if false // make true to enable hooks/extensions

using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

#if !UNITY_EDITOR
#error ConsoleE_Extensions.cs is being compiled in non-editor build // ConsoleE is an editor-only extension
#endif

namespace ConsoleE
{
	/// <summary>
	/// This sample class is provided to show you how to install hooks to extend some aspects of ConsoleE
	/// </summary>
	[InitializeOnLoad]
	public class ConsoleE_Extensions
	{
		static ConsoleE_Extensions()
		{
			// every time you press Run, hooks need to be resync-ed with ConsoleE.
			// with [InitializeOnLoad], this register step will occur whenever you press play
			// only += is needed, no need for -= later
			// most events require ConsoleE Pro
			ConsoleE.Api.onClick += OnClick;   // event is triggered when user tries to open a file
			ConsoleE.Api.onBuildMenu += OnBuildMenu; // event is triggered when user opens a menu
			ConsoleE.Api.onSelection += OnSelection; // single item in the main area has been selected, or selection in the callstack has changes
			ConsoleE.Api.onFormatObject += FormatObjectText;// for formatting Object.name
			ConsoleE.Api.onFormatScript += FormatScriptText;// for formatting script filename
			ConsoleE.Api.onFormatCallstackEntry += FormatScriptCallstackEntry;// for formatting primary callstack row
			// these hooks are available in ConsoleE free:
			ConsoleE.Api.onFormatTime += FormatTime; // for formatting Time.time
			ConsoleE.Api.onFormatFrame += FormatFrameCount; // for formatting Time.frameCount
		}

		/// <summary>
		/// Called whenever a file is about to be opened by the console.  In this method, you have the option of overriding the open file behavior.
		/// Requires ConsoleE PRO.
		/// </summary>
		/// <param name="p">If p.OpenUsingDefaultMethod is true, ConsoleE will open the file normally.  If false, ConsoleE will no nothing.
		/// p.windowArea describes if the click ocurred in the main area or in the callstack.
		/// p.logEntry contains info about the selected log entry. If multi entries are selected, p.logEntry is null.
		/// p.logEntry.tabCategory is the tab category for the log item clicked.
		/// p.logEntry.rows are the rows of the log entry. Rows[0] is the Log msg, the rest is the callstack.
		/// p.filename is filename that should be open (considering wrapper list).
		/// p.externalEditorPath if user used "Open with", this indicates which editor should be used to open the file.
		/// p.indexOpenWithOption if user used "Open with, this is the index of the menu item in "Open with" that was clicked.
		/// p.indexRowSelected, which callstack row was clicked, or -1 if none.
		/// </param>
		/// <returns></returns>
		static public void OnClick(OnClickParams p)
		{
			string selectedEntryText = string.Empty;
			if(p.logEntry != null && p.logEntry.rows.Length > 0)
			{
				selectedEntryText = p.logEntry.rows[0];
				if(selectedEntryText.Length > 40)
					selectedEntryText = selectedEntryText.Substring(0, 35) + "...";
			}
			UnityEngine.Debug.Log(string.Format("OnClick: {0}\nLine: {1}, Editor:{2}, Callstack index selected:{3}, Area:{4}\nSelected entry text: {5}", p.filename, p.lineNumber, p.externalEditorPath, p.indexRowSelected, p.windowArea, selectedEntryText));
			p.openUsingDefaultMethod = true; // true if not handled in which case ConsoleE will open the file using the default code, set to false if you do not want ConsoleE to open the file
		}

		/// <summary>
		/// Call when a single item is selected, or if a single item is deselected, or if a select event occurs in the callstack
		/// </summary>
		/// <param name="p">
		/// p.windowArea describes if the selection ocurred in the main area or in the callstack.
		/// p.logEntry contains info about the selected log entry. If multi entries are selected, p.logEntry is null.
		/// p.logEntry.tabCategory is the tab category for the log item clicked.
		/// p.logEntry.rows are the rows of the log entry. Rows[0] is the Log msg, the rest is the callstack.
		/// </param>
		private static void OnSelection(OnSelectionParams p)
		{
			var e = p.logEntry;
			if(e != null)
			{
				if(p.windowArea == WindowAreas.mainArea)
				{
					e.SelectObjectInUnity();
				}
				else // callstack
				{
					int i = e.indexRowSelected;
					if(i >= 0 && i < e.rows.Length)
					{
						int lineNumber;
						string filename = LogEntry.ParseForFilename(out lineNumber, e.rows[i], true, true);
						if(filename != null)
						{
							UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(filename, typeof(UnityEngine.Object));
							if(obj != null)
							{
								Selection.activeObject = obj;
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Called whenever a context menu is about to be shown. In this method, you have the option of editing the menu before it is shown.
		/// p.Menu is of type GenericMenuEx which is like Unity's GenericMenu but provides extended functionality.
		/// Requires ConsoleE PRO.
		/// </summary>
		/// <param name="p">p.menu is the list of menu items. 
		/// p.windowArea describes what type of menu we are building.  
		/// p.logEntry contains info about the selected log entry. If multi entries are selected, p.logEntry is null.
		/// </param>
		/// <returns></returns>
		static public void OnBuildMenu(OnBuildMenuParams p)
		{
			if(p.windowArea == WindowAreas.toolbar)
				p.menu.AddItem(new GUIContent("Toolbar menu option [HOOK]"), false, OnOptionMenuClicked, p);

			p.menu.AddItem(new GUIContent("Insert at index 1 [HOOK]"), false, OnOptionMenuInserted, p, 1);

			if(p.windowArea == WindowAreas.callstackArea)
			{
				// example of how you can edit the existing default menu list
				var entries = p.menu.Entries;
				for(int i = 0; i < entries.Count; i++)
				{
					if(entries[i].Text.StartsWith("Search Web"))
					{
						p.menu.RemoveAt(i);
						break;
					}
				}
			}

			p.menu.AddSeparator(string.Empty);

			if(p.logEntry != null && p.logEntry.rows.Length > 0)
			{
				// example of using info from the selected entry
				string text = p.logEntry.rows[0];
				if(text.Length > 20)
					text = text.Substring(0, 15) + "...";
				p.menu.AddItem(new GUIContent("Selected Entry Info [HOOK]"), false, OnOptionMenuClicked, p);
			}
			else
			{
				p.menu.AddDisabledItem(new GUIContent("Selected Entry Info [HOOK]"));
			}
			
			p.showWindow = true; // set this to true to tell ConsoleE to show the menu.  This is ignored if Unity is building the menu for IHasCustomMenu.
		}

		private static void OnOptionMenuInserted(object userData)
		{
			var p = (OnBuildMenuParams) userData;
			UnityEngine.Debug.Log("Menu option clicked: inserted entry from " + p.windowArea.ToString());
		}

		static void OnOptionMenuClicked(object userData)
		{
			var p = (OnBuildMenuParams) userData;

			UnityEngine.Debug.Log(string.Format("Menu option clicked: selected entry (tab={0} index={1})", p.logEntry.tabCategory, p.logEntry.indexUnity));

			string s = p.logEntry.GetCallstackPartialTextSelection();
			if(s != null)
			{
				UnityEngine.Debug.Log("Partially selected text in callstack:\n[START]\n" + s + "\n[END]\n");
			}
		}

		/// <summary>
		/// For overriding how the time column is displayed.  ConsoleE optionally can display Time.time for when Debug.Log() is called.  This hook allows you to format how time is displayed.
		/// For ConsoleE FREE & PRO.
		/// </summary>
		/// <param name="p">p.area is the area in console where this item is being rendered</param>
		/// <param name="time">value of Time.time when Debug.Log() was called, -1 if app not running</param>
		/// <param name="isTimeApproximate">true if the console was not able to determine exactly when the log message was created.  This can happen when a log entry is added before the ConsoleE window is enabled or if a Debug.Log() occurs during Application.logMessageReceived.</param>
		/// <returns>the time string, do not return null</returns>
		static string FormatTime(OnFormatItemParams p, float time, bool isTimeApproximate)
		{
			if(time >= 0)
			{
				string s = !isTimeApproximate ? time.ToString("0.000") : time.ToString("~0.000");
				if(p.area != ExtraItemArea.callstack)
					return s;
				return "Time: " + s;
			}
			return string.Empty; // log while app not running
		}

		/// <summary>
		/// For overriding how the frame count column is displayed.  ConsoleE optionally can display Time.frameCount for when Debug.Log() is called.  This hook allows you to format how frame count value is displayed.
		/// For ConsoleE FREE & PRO.
		/// </summary>
		/// <param name="p">p.area is the area in console where this item is being rendered</param>
		/// <param name="frameCount">value of Time.frameCount when Debug.Log() was called, -1 if app is not running</param>
		/// <param name="isFrameCountApproximate">true if the console was not able to determine exactly when the log message was created.  This can happen when a log entry is added before the ConsoleE window is enabled or if a Debug.Log() occurs during Application.logMessageReceived.</param>
		/// <returns>the frame count string, do not return null</returns>
		static string FormatFrameCount(OnFormatItemParams p, int frameCount, bool isFrameCountApproximate)
		{
			if(frameCount >= 0)
			{
				string s = !isFrameCountApproximate ? frameCount.ToString() : frameCount.ToString("~0");
				if(p.area != ExtraItemArea.callstack)
					return s;
				return "Frame: " + s;
			}
			return string.Empty; // log while app not running
		}

		/// <summary>
		/// For overriding how the "object" column is displayed.  ConsoleE optionally can display the name of the object passed to Debug.Log().  This hook allows you to format how the string is displayed in the console.
		/// For ConsoleE PRO.
		/// </summary>
		/// <param name="p">p.area is the area in console where this item is being rendered</param>
		/// <param name="objectName">Name of the object being displayed</param>
		/// <returns>object text display string, do not return null</returns>
		static string FormatObjectText(OnFormatItemParams p, string objectName)
		{
			if(p.area != ExtraItemArea.callstack)
				return objectName;
			return "Object: " + objectName;
		}

		/// <summary>
		/// For overriding how the "script" column is displayed.  ConsoleE optionally can display the filename associated with the Debug.Log() call.  This hook allows you to format how the string is displayed in the console.
		/// For ConsoleE PRO.
		/// </summary>
		/// <param name="p">p.area is the area in console where this item is being rendered</param>
		/// <param name="filename">Full filename of the script file</param>
		/// <param name="hideExtension">user has clicked checkbox to hide extension</param>
		/// <returns>filename string to display, do not return null</returns>
		static string FormatScriptText(OnFormatItemParams p, string filename, bool hideExtension)
		{
			string s = hideExtension ? System.IO.Path.GetFileNameWithoutExtension(filename) : System.IO.Path.GetFileName(filename);
			if(p.area != ExtraItemArea.callstack)
				return s;
			return "Script: " + s;
		}

		/// <summary>
		/// For overriding how the "callstack entry" column is displayed.  The callstack entry is the primary row in the callstack, skipping past any wrapper functions.
		/// For ConsoleE PRO.
		/// </summary>
		/// <param name="p">p.area is the area in console where this item is being rendered</param>
		/// <param name="callstackEntry">text for the row in the callstack that ConsoleE has deemed primary</param>
		/// <param name="indexRow">index of the row in the callstack that ConsoleE has deemed primary, -1 if ConsoleE does not find a callstack row</param>
		/// <returns>filename string to display, do not return null</returns>
		static string FormatScriptCallstackEntry(OnFormatItemParams p, string callstackEntry, int indexRow)
		{
			if(indexRow == -1) // if no primary callstack row
				return string.Empty;
			if(indexRow >= 0 && indexRow < p.logEntry.rows.Length)
				return p.logEntry.rows[indexRow];
			return callstackEntry;
		}
	}
}
#endif
