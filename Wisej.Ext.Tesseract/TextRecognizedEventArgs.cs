﻿///////////////////////////////////////////////////////////////////////////////
//
// (C) 2023 ICE TEA GROUP LLC - ALL RIGHTS RESERVED
//
// Author: Levie Rufenacht
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

namespace Wisej.Ext.Tesseract
{
	/// <summary>
	/// Event arguments for a text recognition event.
	/// </summary>
	public class TextRecognizedEventArgs : EventArgs
	{
		/// <summary>
		/// Creates a new instance of <see cref="TextRecognizedEventArgs"/> with the given configuration.
		/// </summary>
		/// <param name="confidence">The confidence of the recognition. Generally a higher confidence indicates that the result is more accurate.</param>
		/// <param name="text">The text discovered.</param>
		/// <param name="words">An array of strings containing the words discovered.</param>
		public TextRecognizedEventArgs(int confidence, string text, string[] words)
		{
			this.Confidence = confidence;
			this.Words = words;
			this.Text = text;
		}

		/// <summary>
		/// Returns the confidence level of the scan event.
		/// </summary>
		/// <remarks>
		/// Generally a higher confidence indicates that the result is more accurate.
		/// </remarks>
		public int Confidence { get; }

		/// <summary>
		/// Returns the text detected during the scan event.
		/// </summary>
		public string Text { get; }

		/// <summary>
		/// Returns a list of words that were detected during the scan event.
		/// </summary>
		public string[] Words { get; }

	}
}
