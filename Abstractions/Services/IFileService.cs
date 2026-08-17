using System;
using System.Collections.Generic;
using System.Text;

namespace Abstractions.Services
{
	public interface IFileService
	{
		Task<string> SaveFileAsync(Stream fileStream, string fileName, string subfolder, CancellationToken cancellationToken = default);
		Task<(Stream FileStream, string ContentType)?> GetFileAsync(string filePath, CancellationToken cancellationToken = default);
		Task DeleteFileAsync(string filePath, CancellationToken cancellationToken = default);
	}
}
