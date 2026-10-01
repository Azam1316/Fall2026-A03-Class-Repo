namespace Day07Demo.Features.Cart.Store
{
    public static class CartGuard
    {
        public static bool IsCartEmpty(CartState state)
        {
            return state.Items.Count == 0;
        }
    }
}
