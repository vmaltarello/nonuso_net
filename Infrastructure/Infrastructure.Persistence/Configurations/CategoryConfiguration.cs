using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nonuso.Domain.Entities;

namespace Nonuso.Infrastructure.Persistence.Configurations
{
    internal class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasData(
                new Category { Id = Guid.Parse("F4D0E95E-21B0-4F85-8894-15A09FC7DE6B"), Description = "Abbigliamento e Accessori", DescriptionEN = "Clothing and Accessories" },
                new Category { Id = Guid.Parse("7624F3A2-39CA-4986-8033-E7E7F1C29003"), Description = "Accessori Auto", DescriptionEN = "Car Accessories" },
                new Category { Id = Guid.Parse("3FB6FCEE-DAA4-43E0-8CC3-853E64F391B7"), Description = "Accessori Moto", DescriptionEN = "Motorcycle Accessories" },
                new Category { Id = Guid.Parse("0BE13E80-BDB2-448D-954F-43606C0B4F1C"), Description = "Altro", DescriptionEN = "Other" },
                new Category { Id = Guid.Parse("43DFF0BB-C45F-4AA1-9E2E-ED561B08CC9B"), Description = "Arredamento e Casalinghi", DescriptionEN = "Furniture and Household" },
                new Category { Id = Guid.Parse("88AE11D7-E7E7-4CC0-A12B-0C0363409143"), Description = "Audio/Video", DescriptionEN = "Audio/Video" },
                new Category { Id = Guid.Parse("8F6282A5-8D63-49C1-A57E-E66F3C29A768"), Description = "Auto", DescriptionEN = "Cars" },
                new Category { Id = Guid.Parse("493FAE16-51FC-4BA9-8925-226AA3363E5C"), Description = "Biciclette", DescriptionEN = "Bicycles" },
                new Category { Id = Guid.Parse("606B6FAC-AE13-4317-B1D6-CD217A9B05A4"), Description = "Caravan e Camper", DescriptionEN = "Caravan and Motorhome" },
                new Category { Id = Guid.Parse("8558B4CB-8A92-4244-8339-96B1E1568A77"), Description = "Collezionismo", DescriptionEN = "Collectibles" },
                new Category { Id = Guid.Parse("56A1FBEC-207A-4904-BDF5-7171CEF95DD4"), Description = "Console e Videogiochi", DescriptionEN = "Gaming Consoles and Video Games" },
                new Category { Id = Guid.Parse("E780280E-527B-4042-9473-6FEF1FDF8FFB"), Description = "Elettrodomestici", DescriptionEN = "Home Appliances" },
                new Category { Id = Guid.Parse("29BC1B7D-3195-4FB2-B513-58044E51E020"), Description = "Elettronica", DescriptionEN = "Electronics" },
                new Category { Id = Guid.Parse("2489FFE6-6F52-4A61-9028-7997C9C62932"), Description = "Fotografia", DescriptionEN = "Photography" },
                new Category { Id = Guid.Parse("177F5CB8-D863-45AC-9F22-9F75C154D4AC"), Description = "Giardino e Fai da Te", DescriptionEN = "Garden and DIY" },
                new Category { Id = Guid.Parse("686293D1-0DAA-4867-940D-9AC02B07F768"), Description = "Informatica", DescriptionEN = "Computing" },
                new Category { Id = Guid.Parse("741CA2E5-D19B-464C-9D77-B5BDC2CC6490"), Description = "Libri e Riviste", DescriptionEN = "Books and Magazines" },
                new Category { Id = Guid.Parse("D3E18DD5-2364-4F9B-9ED7-71A1B07BE62C"), Description = "Moto e Scooter", DescriptionEN = "Motorcycles and Scooters" },
                new Category { Id = Guid.Parse("7ED4459D-C4FF-451B-80CE-6B6CF03F4949"), Description = "Musica e Film", DescriptionEN = "Music and Movies" },
                new Category { Id = Guid.Parse("34CFFF2E-2CAC-4BC3-A9E2-F9DABE555094"), Description = "Nautica", DescriptionEN = "Nautical" },
                new Category { Id = Guid.Parse("5B16363F-89DA-4DF4-BC04-F1A0B7AD9DEB"), Description = "Sport", DescriptionEN = "Sports" },
                new Category { Id = Guid.Parse("79D3DE4D-517C-4840-A83A-0697886D2AE9"), Description = "Strumenti Musicali", DescriptionEN = "Musical Instruments" },
                new Category { Id = Guid.Parse("A49F84C7-CBE3-4AD3-A8A3-014DD7AC8603"), Description = "Telefonia", DescriptionEN = "Mobile Phones" },
                new Category { Id = Guid.Parse("2E55268E-F134-42FE-922C-19335025F372"), Description = "Tutto motori", DescriptionEN = "All Motors" },
                new Category { Id = Guid.Parse("BB1A3695-6580-409C-AC18-4A943604E7F8"), Description = "Tutto per i bambini", DescriptionEN = "Kids Items" },
                new Category { Id = Guid.Parse("EC7B4635-DBE2-4EBB-8405-1935602FC31F"), Description = "Veicoli", DescriptionEN = "Vehicles" },
                new Category { Id = Guid.Parse("8EC503C0-2E04-43E7-B940-3FFC99BC855A"), Description = "Accessori per animali", DescriptionEN = "Pet Supplies" }
            );
        }
    }
}
