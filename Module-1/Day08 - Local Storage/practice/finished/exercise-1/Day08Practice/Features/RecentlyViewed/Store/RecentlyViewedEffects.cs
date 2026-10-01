using Blazored.LocalStorage;
using Day08Practice.Models;
using Fluxor;

namespace Day08Practice.Features.RecentlyViewed.Store;

public class RecentlyViewedEffects(
    ILocalStorageService localStorage,
    IState<RecentlyViewedState> recentState)
{
    private const string RecentlyViewedStorageKey = "recentlyViewed";

    // Practice 1 Solution -- Save on change.
    // The effect runs AFTER the reducer, so recentState.Value.Items is already the updated,
    // de-duplicated, capped-at-5 list. Save the list from state, not action.Item.
    [EffectMethod]
    public async Task HandleViewProductAction(ViewProductAction action, IDispatcher dispatcher)
    {
        await localStorage.SetItemAsync(RecentlyViewedStorageKey, recentState.Value.Items);
    }

    // Practice 1 Solution -- Rehydrate on startup.
    // "Rehydrate" = restore the saved list from localStorage when the app starts. The effect
    // does the async read (a reducer can't -- it's pure/synchronous), then dispatches
    // HydrateRecentlyViewedAction so the reducer can copy the loaded items into state.
    [EffectMethod]
    public async Task HandleHydrateRecentlyViewedRequestAction(HydrateRecentlyViewedRequestAction action, IDispatcher dispatcher)
    {
        try
        {
            var items = await localStorage.GetItemAsync<List<RecentlyViewedItem>>(RecentlyViewedStorageKey);
            if (items is not null && items.Count > 0)
            {
                dispatcher.Dispatch(new HydrateRecentlyViewedAction(items));
            }
        }
        catch
        {
            // Blazored.LocalStorage throws JsonException on corrupt or mismatched JSON.
            // Bare catch is intentional here: any exception reading localStorage means the
            // data is unrecoverable -- the correct response in all failure modes is to clear
            // the key and start fresh.
            // Do not use bare catch in effects that have specific recoverable failure modes;
            // use the specific exception type there (e.g., catch (JsonException)).
            await localStorage.RemoveItemAsync(RecentlyViewedStorageKey);
        }
    }
}
