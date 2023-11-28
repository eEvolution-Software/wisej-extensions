﻿///////////////////////////////////////////////////////////////////////////////
//
// (C) 2015 ICE TEA GROUP LLC - ALL RIGHTS RESERVED
//
// 
//
// ALL INFORMATION CONTAINED HEREIN IS, AND REMAINS
// THE PROPERTY OF ICE TEA GROUP LLC AND ITS SUPPLIERS, IF ANY.
// THE INTELLECTUAL PROPERTY AND TECHNICAL CONCEPTS CONTAINED
// HEREIN ARE PROPRIETARY TO ICE TEA GROUP LLC AND ITS SUPPLIERS
// AND MAY BE COVERED BY U.S. AND FOREIGN PATENTS, PATENT IN PROCESS, AND
// ARE PROTECTED BY TRADE SECRET OR COPYRIGHT LAW.
//
// DISSEMINATION OF THIS INFORMATION OR REPRODUCTION OF THIS MATERIAL
// IS STRICTLY FORBIDDEN UNLESS PRIOR WRITTEN PERMISSION IS OBTAINED
// FROM ICE TEA GROUP LLC.
//
///////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using Wisej.Base;
using Wisej.Core;
using Wisej.Design;
using WinForms = System.Windows.Forms;

namespace Wisej.Web.Ext.TinyMCE6
{
	/// <summary>
	/// TinyMCE gives you total control over your rich text editing.
	/// from: https://www.tinymce.com/
	/// </summary>
	[ApiCategory("TinyMCE")]
	[ToolboxItem(true)]
	[ToolboxBitmap(typeof(WinForms.Control), "RichTextBox.bmp")]
	[DefaultProperty("Text")]
	[DefaultEvent("TextChanged")]
	public class TinyMCE : Widget, IWisejControl
	{
		// indicates that the control is ready to update its content.
		private bool initialized;

		#region Events

		/// <summary>
		/// Fired after the editor executes a command.
		/// </summary>
		public event CommandEventHandler Command
		{
			add { base.AddHandler(nameof(Command), value); }
			remove { base.RemoveHandler(nameof(Command), value); }
		}

		/// <summary>
		/// Fires the Command event.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnCommand(CommandEventArgs e)
		{
			((CommandEventHandler)base.Events[nameof(Command)])?.Invoke(this, e);
		}

		#endregion

		#region Properties

		/// <summary>
		/// Returns or sets the HTML text associated with this control.
		/// </summary>
		/// <returns>The HTML text associated with this control.</returns>
		[DefaultValue("")]
		public override string Text
		{
			get
			{
				return this._text;
			}
			set
			{
				value = value ?? string.Empty;

				if (this._text != value)
				{
					this._text = value;
					OnTextChanged(EventArgs.Empty);

					if (!((IWisejControl)this).IsNew /* cannot call setText until the widget is created.*/ )
						Call("setText", TextUtils.EscapeText(value, true));
				}
			}
		}
		private string _text = "";

		/// <summary>
		/// Shows or hides the menu panel.
		/// </summary>
		[DesignerActionList]
		[DefaultValue(true)]
		[Description("Shows or hides the menu panel.")]
		public bool ShowMenuBar
		{
			get { return this._showMenuBar; }

			set
			{
				if (this._showMenuBar != value)
				{
					this._showMenuBar = value;
					Update();
				}
			}
		}
		private bool _showMenuBar = true;

		/// <summary>
		/// Shows or hides the toolbar panel.
		/// </summary>
		[DesignerActionList]
		[DefaultValue(true)]
		[Description("Shows or hides the toolbar panel.")]
		public bool ShowToolbar
		{
			get { return this._showToolbar; }

			set
			{
				if (this._showToolbar != value)
				{
					this._showToolbar = value;
					Update();
				}
			}
		}
		private bool _showToolbar = true;

		/// <summary>
		/// Shows or hides the footer panel.
		/// </summary>
		[DesignerActionList]
		[DefaultValue(true)]
		[Description("Shows or hides the footer panel.")]
		public bool ShowFooter
		{
			get { return this._showFooter; }

			set
			{
				if (this._showFooter != value)
				{
					this._showFooter = value;
					Update();
				}
			}
		}
		private bool _showFooter = true;

		/// <summary>
		/// Returns or sets the configuration to use for this instance of the editor: <see href="https://www.tinymce.com/docs/configure/"/>.
		/// </summary>
		[DesignerActionList]
		[MergableProperty(false)]
		[Editor("Wisej.Design.CodeEditor, Wisej.Framework.Design, Version=3.0.0.0, Culture=neutral, PublicKeyToken=17bef35e11b84171", 
				"System.Drawing.Design.UITypeEditor, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		public new virtual dynamic Options
		{
			get
			{
				return this._options;
			}
			set
			{
				this._options = value;
				Update();
			}
		}
		private dynamic _options = new DynamicObject();

		/// <summary>
		/// Returns or sets the font names to display in the toolbar.
		/// </summary>
		[DesignerActionList]
		[TypeConverter(typeof(ArrayConverter))]
		[Description("Returns or sets the font names to display in the toolbar.")]
		[Editor("System.Windows.Forms.Design.StringArrayEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", 
				"System.Drawing.Design.UITypeEditor, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		public string[] FontNames
		{
			get { return this._fontNames; }
			set
			{
				if (this._fontNames != value)
				{
					this._fontNames = value;
					Update();
				}
			}
		}
		private string[] _fontNames = DefaultFontNames;

		private bool ShouldSerializeFontNames()
		{
			return this._fontNames != DefaultFontNames;
		}

		private void ResetFontNames()
		{
			this._fontNames = DefaultFontNames;
			Update();
		}

		/// <summary>
		/// Returns the default buttons to show in the toolbar.
		/// </summary>
		public static string[] DefaultFontNames
		{
			get
			{
				return _defaultFontNames;
			}
		}
		private static string[] _defaultFontNames = new[] { "Verdana", "Arial", "Georgia", "Trebuchet MS" };

		/// <summary>
		/// Collection of external (local) plugins to register with the TinyMCE control.
		/// </summary>
		[DefaultValue(null)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ExternalPlugin[] ExternalPlugins
		{
			get
			{
				return this._externalPlugins;
			}
			set
			{
				if (value != this._externalPlugins)
				{
					this._externalPlugins = value;
					Update();
				}
			}
		}
		private ExternalPlugin[] _externalPlugins;

		#endregion

		#region Methods

		/// <summary>
		/// Executes commands to manipulate the contents of the editable region. 
		/// </summary>
		/// <param name="command">The name of the command to execute. See <see href="https://developer.mozilla.org/en-US/docs/Web/API/Document/execCommand and http://archive.tinymce.com/wiki.php/TinyMCE3x:Command_identifiers"/> for a list of commands.</param>
		/// <param name="showDefaultUI">Indicates whether the default user interface should be shown. This is not implemented in Mozilla.</param>
		/// <param name="argument">For commands which require an input argument (such as insertImage, for which this is the URL of the image to insert), this is a string providing that information. Specify null if no argument is needed.</param>
		/// <remarks>
		/// Most commands affect the document's selection (bold, italics, etc.), while others insert new elements (adding a link) or 
		/// affect an entire line (indenting). When using contentEditable, calling execCommand() will affect the 
		/// currently active editable element.
		/// </remarks>
		public void ExecCommand(string command, bool showDefaultUI = false, string argument = null)
		{
			Call("execCommand", command, showDefaultUI, argument);
		}

		/// <summary>
		/// Executes commands to manipulate the contents of the editable region. 
		/// </summary>
		/// <param name="command">The name of the command to execute. See <see href="https://developer.mozilla.org/en-US/docs/Web/API/Document/execCommand"/> and <see href="http://archive.tinymce.com/wiki.php/TinyMCE3x:Command_identifiers"/> for a list of commands.</param>
		/// <param name="argument">For commands which require an input argument (such as insertImage, for which this is the URL of the image to insert), this is a string providing that information. Specify null if no argument is needed.</param>
		/// <remarks>
		/// Most commands affect the document's selection (bold, italics, etc.), while others insert new elements (adding a link) or 
		/// affect an entire line (indenting). When using contentEditable, calling execCommand() will affect the 
		/// currently active editable element.
		/// </remarks>
		public void ExecCommand(string command, string argument = null)
		{
			ExecCommand(command, false, argument);
		}

		#endregion

		#region Wisej Implementation

		/// <summary>
		/// Returns the designer timeout for rendering this control.
		/// </summary>
		int IWisejControl.DesignerTimeout { get { return 8000; } }

		/// <summary>
		/// Returns the theme appearance key for this control.
		/// </summary>
		string IWisejControl.AppearanceKey
		{
			get { return this.AppearanceKey ?? "tinymce"; }
		}

		/// <summary>
		/// Overridden to create our initialization script.
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string InitScript
		{
			get { return BuildInitScript(); }
			set { }
		}

		/// <summary>
		/// Returns or sets the base url for the TinyMCE installation
		/// </summary>
		public static string BaseUrl
		{
			get { return _baseUrl; }
			set { _baseUrl = value; }
		}
		private static string _baseUrl = "https://cdnjs.cloudflare.com/ajax/libs/tinymce/6.0.3/";

		/// <summary>
		/// Overridden to return our list of script resources.
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override List<Package> Packages
		{
			get
			{
				if (base.Packages.Count == 0)
				{
					// initialize the loader with the required libraries.
					base.Packages.Add(new Package()
					{
						Name = "tinymce.js",
						Source = $"{BaseUrl}tinymce.min.js"
					});
				}
				return base.Packages;
			}
		}

		// disable inlining or we lose the calling assembly in GetResourceString().
		[MethodImpl(MethodImplOptions.NoInlining)]
		private string BuildInitScript()
		{
			IWisejControl me = this;
			dynamic options = new DynamicObject();
			string script = GetResourceString("Wisej.Web.Ext.TinyMCE6.JavaScript.startup.js");

			options.config = this.Options;
			options.fonts = this.FontNames;
			options.showFooter = this.ShowFooter;
			options.showMenubar = this.ShowMenuBar;
			options.showToolbar = this.ShowToolbar;
			options.externalPlugins = this.ExternalPlugins;
			script = script.Replace("$options", options.ToString());

			return script;
		}

		/// <summary>
		/// Updates the client component using the state information.
		/// </summary>
		/// <param name="state">Dynamic state object.</param>
		protected override void OnWebUpdate(dynamic state)
		{
			if (state.text != null && this.initialized)
			{
				if (this._text != state.text)
				{
					this._text = state.text;
					OnTextChanged(EventArgs.Empty);
				}
			}

			state.Delete("text");

			base.OnWebUpdate((object)state);
		}

		/// <summary>
		/// Fires the <see cref="E:Wisej.Web.Control.WidgetEvent" /> event.
		/// </summary>
		/// <param name="e">A <see cref="T:Wisej.Web.WidgetEventArgs" /> that contains the event data. </param>
		protected override void OnWidgetEvent(WidgetEventArgs e)
		{
			switch (e.Type)
			{
				case "load":
					this.initialized = true;
					if (!String.IsNullOrEmpty(this.Text))
						Call("setText", TextUtils.EscapeText(this.Text, true));
					break;

				case "changeText":
					this.Text = e.Data ?? "";
					break;

				case "command":
					ProcessCommandWebEvent(e);
					break;

				case "focus":
					ProcessFocusWebEvent(e);
					break;
			}
			base.OnWidgetEvent(e);
		}

		// Handles the "focus" event from the client.
		private void ProcessFocusWebEvent(WidgetEventArgs e)
		{
			// HTML editors focus a child IFrame which causes the
			// container widget to lose the focus.
			Focus();

			// activate and bring to top the parent form.
			var form = FindForm();
			if (form != null && !form.Active)
				form.Activate();
		}

		// Handles the "command" event from the client.
		private void ProcessCommandWebEvent(WidgetEventArgs e)
		{
			OnCommand(new CommandEventArgs(e.Data));
		}

		#endregion
	}
}
