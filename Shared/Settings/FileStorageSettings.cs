using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Settings
{
	public class FileStorageSettings
	{
		public const string SectionName = "FileStorage";
		public string BasePath { get; set; } = string.Empty;
		public long MaxFileSizeBytes { get; set; } = 5 * 1024 * 1024; // 5 mb
		public string[] AllowedExtensions { get; set; } = [".jpg", ".jpeg", ".png"];
	}
}
