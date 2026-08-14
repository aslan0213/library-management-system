using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Applications;
using Shared.Dtos.Notification;
using Shared.Paging;

namespace LibraryManagement.Api.Controllers
{
	/// <summary>
	///	Manages member notifications.
	/// </summary>
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class NotificationController : ControllerBase
	{
		private readonly INotificationAppService _notificationAppService;

		public NotificationController(INotificationAppService notificationAppService)
		{
			_notificationAppService = notificationAppService;
		}

		/// <summary>
		/// Gets a paged, sortable list of notifications.
		/// </summary>
		[HttpGet]
		[ProducesResponseType(typeof(PagedResult<NotificationResponse>), StatusCodes.Status200OK)]
		public async Task<ActionResult<PagedResult<NotificationResponse>>> GetPaged(
			[FromQuery] PagedRequest request,
			CancellationToken cancellationToken)
		{
			var result = await _notificationAppService.GetPagedAsync(request, cancellationToken);
			return Ok(result);
		}

		/// <summary>
		/// Gets a single notification by Id.
		/// </summary>
		[HttpGet("{id:guid}")]
		[ProducesResponseType(typeof(NotificationResponse), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<NotificationResponse>> GetById(Guid id, CancellationToken cancellationToken)
		{
			var notification = await _notificationAppService.GetByIdAsync(id, cancellationToken);
			if(notification is null)
			{
				return NotFound();
			}
			return Ok(notification);
		}

		/// <summary>
		/// Marks a notification as read.
		/// </summary>
		[HttpPut("{id:guid}/read")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> MarkAsRead(Guid id, CancellationToken cancellationToken)
		{
			await _notificationAppService.MarkAsReadAsync(id, cancellationToken);
			return NoContent();
		}
	}
}
