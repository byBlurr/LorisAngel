using Microsoft.AspNetCore.Components;

namespace LorisAngel.Webpanel.Data
{
    public class RedirectToLogin : ComponentBase
    {
        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
            NavigationManager.NavigateTo("/login", true);
        }
    }
}
