using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVCMusicStore.Data;
using System.Threading.Tasks;

namespace MVCMusicStore.ViewComponents
{
    public class GenreViewComponent : ViewComponent
    {
        private readonly StoreDbContext storeDbContext;

        public GenreViewComponent(StoreDbContext storeDbContext)
        {
            this.storeDbContext = storeDbContext;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var genres = await storeDbContext.Genres.ToListAsync();
            return View(genres);
        }
    }
}
