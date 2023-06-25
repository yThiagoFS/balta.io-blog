using Blog.Data;
using Blog.Extensions;
using Blog.Models;
using Blog.ViewModels;
using Blog.ViewModels.Categories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly BlogDataContext _context;

        public CategoryController(
            [FromServices] BlogDataContext context )
        {
            _context = context;
        }

        [HttpGet("health-check")]
        public IActionResult HealthCheck() => Ok();

        [HttpGet()]
        public async Task<IActionResult> GetAsync()
        {
            try
            {
                var categories = await _context.Categories.ToListAsync();

                return Ok(new ResultViewModel<List<Category>>(categories));
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<string>("05XE7 - Falha interna no servidor"));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute]int id)
        {
            try
            {
                var category = await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);

                return category != null
                    ? Ok(new ResultViewModel<Category>(category))
                    : NotFound(new ResultViewModel<Category>("Categoria não encontrada."));
            }
            catch 
            {
                return StatusCode(500, new ResultViewModel<Category>("05XE8 - Falha interna no servidor"));
            }
        }

        [HttpPost()]
        public async Task<IActionResult> CreateAsync([FromBody] EditorCategoryViewModel categoryVm) 
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResultViewModel<Category>(ModelState.GetErrors()));

            try 
            {
                var category = new Category
                {
                    Id = 0,
                    Name = categoryVm.Name,
                    Slug = categoryVm.Slug.ToLower(),
                };

                await _context.Categories.AddAsync(category);
                await _context.SaveChangesAsync();

                return Created($"{category.Id}", new ResultViewModel<Category>(category));
            }
            catch(DbUpdateException ex) 
            {
                return StatusCode(500, new ResultViewModel<Category>("O5XE9 - Não foi possível incluir a categoria"));
            }
            catch(Exception ex)
            {
                return StatusCode(500, new ResultViewModel<Category>("O5XE10 - Falha interna no servidor"));
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(
                [FromRoute]int id
                ,[FromBody] EditorCategoryViewModel categoryVm)
        {
            try
            {
                var categoryDb = await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);

                if (categoryDb == null) return NotFound(new ResultViewModel<Category>("Categoria não encontrada."));

                categoryDb.Name = categoryVm.Name;
                categoryDb.Slug = categoryVm.Slug;

                _context.Categories.Update(categoryDb);
                await _context.SaveChangesAsync();

                return Ok(new ResultViewModel<Category>(categoryDb));
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new ResultViewModel<Category>("O5XE11 - Não foi possível atualizar a categoria"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultViewModel<Category>("O5XE12 - Falha interna no servidor"));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(
                [FromRoute] int id)
        {
            try
            {
                var categoryDb = await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);

                if (categoryDb == null) return NotFound(new ResultViewModel<Category>("Categoria não encontrada."));

                _context.Categories.Remove(categoryDb);
                await _context.SaveChangesAsync();


                return Ok(new ResultViewModel<Category>(categoryDb));
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new ResultViewModel<Category>("O5XE13 - Não foi possível deletar a categoria"));
            }
            catch (Exception ex)
            {
                return StatusCode(500,  new ResultViewModel<Category>("O5XE14 - Falha interna no servidor"));
            }
        }

    }
}
