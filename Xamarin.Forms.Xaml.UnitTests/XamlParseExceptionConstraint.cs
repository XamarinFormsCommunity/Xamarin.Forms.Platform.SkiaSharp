using System;
using NUnit.Framework.Constraints;

namespace Xamarin.Forms.Xaml.UnitTests
{	
	public class XamlParseExceptionConstraint : ExceptionTypeConstraint
	{
		bool haslineinfo;
		int linenumber;
		int lineposition;
		Func<string, bool> messagePredicate;

		XamlParseExceptionConstraint (bool haslineinfo) : base (typeof (XamlParseException))
		{
			this.haslineinfo = haslineinfo;
			DisplayName = "xamlparse";
		}

		public XamlParseExceptionConstraint () : this (false)
		{
		}

		public XamlParseExceptionConstraint (int linenumber, int lineposition, Func<string, bool> messagePredicate = null) : this (true)
		{
			this.linenumber = linenumber;
			this.lineposition = lineposition;
			this.messagePredicate = messagePredicate;
		}

		public override bool Matches (object actual)
		{
			this.actual = actual;
			if (!base.Matches (actual))
				return false;
			var xmlInfo = ((XamlParseException)actual).XmlInfo;
			if (!haslineinfo)
				return true;
			if (xmlInfo == null || !xmlInfo.HasLineInfo ())
				return false;
			if (messagePredicate != null)
				if (!messagePredicate (((XamlParseException)actual).UnformattedMessage))
					return false;
			return xmlInfo.LineNumber == linenumber && xmlInfo.LinePosition == lineposition;
		}

		public override void WriteDescriptionTo (MessageWriter writer)
		{
			base.WriteDescriptionTo (writer);
			if (haslineinfo)
				writer.Write (string.Format (" line {0}, position {1}", linenumber, lineposition));
		}

		public override void WriteActualValueTo (MessageWriter writer)
		{
			var ex = actual as XamlParseException;
			writer.WriteActualValue ((actual == null) ? null : actual.GetType ());
			if (ex != null) {
				if (ex.XmlInfo != null && ex.XmlInfo.HasLineInfo ())
					writer.Write (" line {0}, position {1}", ex.XmlInfo.LineNumber, ex.XmlInfo.LinePosition);
				else 
					writer.Write (" no line info");
				writer.WriteLine (" ({0})", ex.Message);
				writer.Write (ex.StackTrace);
			}
		}
	}
}