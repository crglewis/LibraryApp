using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace LibraryApp.Hubs
{
    // Server-to-client broadcast only: clients connect and listen for
    // "BookAvailabilityChanged" events, they never invoke methods on this hub.
    [Authorize]
    public class BookHub : Hub
    {
    }
}
