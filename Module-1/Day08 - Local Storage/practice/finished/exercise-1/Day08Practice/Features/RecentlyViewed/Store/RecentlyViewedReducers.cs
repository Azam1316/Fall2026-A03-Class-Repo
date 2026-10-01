using Fluxor;

namespace Day08Practice.Features.RecentlyViewed.Store;

public static class RecentlyViewedReducers
{
    // The newest-first cap. Named so the reducer and the persisted-list invariant
    // share one source of truth -- the hydration round-trip must keep the same cap.
    private const int MaxRecent = 5;

    // PRE-BUILT (domain reducer): prepend the just-viewed product, drop any earlier
    // duplicate of it, and keep only the most-recent MaxRecent. Pure function, no I/O.
    [ReducerMethod]
    public static RecentlyViewedState ReduceViewProductAction(RecentlyViewedState state, ViewProductAction action)
    {
        var withoutDuplicate = state.Items.Where(i => i.ProductId != action.Item.ProductId);
        var updated = new[] { action.Item }.Concat(withoutDuplicate).Take(MaxRecent).ToList();
        return state with { Items = updated };
    }

    // Practice 1 Solution: copy the loaded items into state. Same one-line shape as the
    // cart's hydration reducer -- only the state/action types differ.
    [ReducerMethod]
    public static RecentlyViewedState ReduceHydrateRecentlyViewedAction(RecentlyViewedState state, HydrateRecentlyViewedAction action)
    {
        return state with { Items = action.Items };
    }
}
