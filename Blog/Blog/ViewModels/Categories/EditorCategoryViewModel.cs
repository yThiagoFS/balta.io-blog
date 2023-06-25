using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModels.Categories
{
    public class EditorCategoryViewModel
    {
        [Required(ErrorMessage = "O campo NAME é obrigatório.")]
        [StringLength(40, MinimumLength = 3, ErrorMessage = "Campo deve ter entre 3 e 40 caracteres.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "O campo SLUG é obrigatório.")]
        public string Slug { get; set; }
    }
}
