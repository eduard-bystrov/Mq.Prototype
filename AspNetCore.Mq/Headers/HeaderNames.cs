using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Mq
{
	public static class HeaderNames
	{
		internal static class Consts
		{
			public const String Method = "X-Compat-Method";
			public const String Path = "X-Compat-Path";
			public const String QueryString = "X-Compat-Query";
			public const String RelativeUri = "X-Compat-Relative-URI";
			public const String ContentLength = "Content-Length";
			public const String RequestId = "Request-Id";

		}

		public static String Method => Consts.Method;
		public static String Path => Consts.Path;
		public static String QueryString => Consts.QueryString;
		public static String RelativeUri => Consts.RelativeUri;
		public static String ContentLength => Consts.ContentLength;
		public static String RequestId => Consts.RequestId;
	}
}
