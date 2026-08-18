using Abstractions.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Shared.Settings;
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
		public LocalFileService(IOptions<FileStorageSettings> settings, IHostEnvironment environment)
		{
			var configuredPath = settings.Value.BasePath;
			_basePath = Path.IsPathRooted(configuredPath) ? configuredPath : Path.GetFullPath(Path.Combine(environment.ContentRootPath, configuredPath));

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
