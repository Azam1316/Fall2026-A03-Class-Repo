using Day08Practice.Models;

namespace Day08Practice.Features.RecentlyViewed.Store;

// PRE-BUILT (domain action): the Shop page dispatches this when a product is viewed.
public record ViewProductAction(RecentlyViewedItem Item);

// Practice 1 Solution: Hydration actions
// Same trigger + result shape the cart used in the demo.
public record HydrateRecentlyViewedRequestAction;                              // the trigger -- carries no data
public record HydrateRecentlyViewedAction(IReadOnlyList<RecentlyViewedItem> Items); // carries the loaded items
