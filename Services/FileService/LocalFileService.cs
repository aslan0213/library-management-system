using Abstractions.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.FileService
{
	public class LocalFileService : IFileService
	{
		private readonly string _basePath;
		private static readonly Dictionary<string, string> ContentTypes =
			new(StringComparer.OrdinalIgnoreCase)
			{
				[".jpg"] = "image/jpeg",
				[".jpeg"] = "image/jpeg",
				[".png"] = "image/png"
			};
		public LocalFileService(string basePath)
		{
			_basePath = basePath;
			Directory.CreateDirectory(_basePath);

		}
		public Task DeleteFileAsync(string filePath, CancellationToken cancellationToken = default)
		{
			var fullPath = Path.Combine(_basePath, filePath);
			if (File.Exists(fullPath))
				File.Delete(fullPath);
			return Task.CompletedTask;
		}

		public Task<(Stream FileStream, string ContentType)?> GetFileAsync(string filePath, CancellationToken cancellationToken = default)
		{
			var fullPath = Path.Combine(_basePath, filePath);
			if (!File.Exists(fullPath))
			{
				return Task.FromResult<(Stream, string)?>(null);
			}
			var extension = Path.GetExtension(fullPath);
			var contentType = ContentTypes.GetValueOrDefault(extension, "application/octet-stream");
			Stream stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
			return Task.FromResult<(Stream, string)?>((stream, contentType));
		}

		public async Task<string> SaveFileAsync(Stream fileStream, string fileName, string subfolder, CancellationToken cancellationToken = default)
		{
			var folderPath = Path.Combine(_basePath, subfolder);
			Directory.CreateDirectory(folderPath);
			var filePath = Path.Combine(folderPath, fileName);
			await using var outputStream = new FileStream(fileName, FileMode.Create);
			await fileStream.CopyToAsync(outputStream, cancellationToken);
			return Path.Combine(subfolder, fileName);
		}
	}
}
