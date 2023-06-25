using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Blog.Models;

namespace Blog.Data.Mappings
{
    public class PostMap : IEntityTypeConfiguration<Post> 
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {   
            //Tabela
            builder.ToTable("Post");

            //Chave
            builder.HasKey(x => x.Id);

            //Identity
            builder
                .Property(x => x.Id)
                .ValueGeneratedOnAdd()
                .UseIdentityColumn();

            //Propriedade
            builder
                .Property(x => x.LastUpdateDate)
                .IsRequired()
                .HasColumnType("SMALLDATETIME")
                // .HasDefaultValueSql("GETDATE()");
                .HasDefaultValue(DateTime.Now.ToUniversalTime());

            //Indice
            builder
                .HasIndex(x => x.Slug, "IX_Post_Slug")
                .IsUnique();

            //Relacionamentos
            //One to Many
            builder
                .HasOne(x => x.Author)
                .WithMany(x => x.Posts)
                .HasConstraintName("FK_Post_Author")
                .OnDelete(DeleteBehavior.Cascade);

            //One To Many
            builder
                .HasOne(x => x.Category)
                .WithMany(x => x.Posts)
                .HasConstraintName("FK_Post_Category")
                .OnDelete(DeleteBehavior.Cascade);

            //Many to many
            builder
                .HasMany(x => x.Tags)
                .WithMany(x => x.Posts)
                .UsingEntity<Dictionary<string,object>>(
                    "PostTag", 
                    post => post.HasOne<Tag>()
                    .WithMany()
                    .HasForeignKey("PostId")
                    .HasConstraintName("FK_PostTag_PostId")
                    .OnDelete(DeleteBehavior.Cascade),
                    tag => tag.HasOne<Post>()
                    .WithMany()
                    .HasForeignKey("TagId")
                    .HasConstraintName("FK_PostTag_TagId")
                    .OnDelete(DeleteBehavior.Cascade)
                );
                //Gera uma terceira tabela baseada em um dicionario, com um objeto de Post que possui uma tag com muitos posts, e uma Tag que possui um post com muitas tags
        }
    }
}