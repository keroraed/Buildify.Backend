using Buildify.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Buildify.Repository.Data;

public class StoreContextSeed
{
    public static async Task SeedAsync(StoreContext context)
    {
        // Seed Categories
        if (!await context.Categories.AnyAsync())
        {
            var categories = new List<Category>
            {
                new Category
                {
                    Name = "مواد البناء الأساسية",
                    Description = "أسمنت، طوب، حديد، رمل، زلط"
                },
                new Category
                {
                    Name = "الدهانات والديكورات",
                    Description = "دهانات داخلية وخارجية، ورق حائط، جبس"
                },
                new Category
                {
                    Name = "السباكة والصحي",
                    Description = "مواسير، خلاطات، أحواض، أدوات صحية"
                },
                new Category
                {
                    Name = "الكهرباء والإنارة",
                    Description = "كابلات، مفاتيح، لمبات، لوحات كهربائية"
                },
                new Category
                {
                    Name = "الأخشاب والأبواب",
                    Description = "خشب، أبواب خشبية، شبابيك، باركيه"
                },
                new Category
                {
                    Name = "السيراميك والبورسلين",
                    Description = "سيراميك حوائط وأرضيات، بورسلين، رخام"
                },
                new Category
                {
                    Name = "الأدوات والمعدات",
                    Description = "أدوات يدوية، معدات كهربائية، سقالات"
                },
                new Category
                {
                    Name = "العزل والمواد الكيماوية",
                    Description = "عزل حراري ومائي، مواد لاصقة، مواد معالجة"
                }
            };

            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync();
        }

        // Seed Products
        if (!await context.Products.AnyAsync())
        {
            // Get category IDs
            var categories = await context.Categories.ToListAsync();
            var cat1 = categories.FirstOrDefault(c => c.Name == "مواد البناء الأساسية")?.Id ?? 1;
            var cat2 = categories.FirstOrDefault(c => c.Name == "الدهانات والديكورات")?.Id ?? 2;
            var cat3 = categories.FirstOrDefault(c => c.Name == "السباكة والصحي")?.Id ?? 3;
            var cat4 = categories.FirstOrDefault(c => c.Name == "الكهرباء والإنارة")?.Id ?? 4;
            var cat5 = categories.FirstOrDefault(c => c.Name == "الأخشاب والأبواب")?.Id ?? 5;
            var cat6 = categories.FirstOrDefault(c => c.Name == "السيراميك والبورسلين")?.Id ?? 6;
            var cat7 = categories.FirstOrDefault(c => c.Name == "الأدوات والمعدات")?.Id ?? 7;
            var cat8 = categories.FirstOrDefault(c => c.Name == "العزل والمواد الكيماوية")?.Id ?? 8;

            var products = new List<Product>
            {
                // مواد البناء الأساسية
                new Product
                {
                    Name = "أسمنت بورتلاندي 50 كجم",
                    Description = "أسمنت بورتلاندي عادي مطابق للمواصفات المصرية",
                    Price = 180.00m,
                    Stock = 1000,
                    CategoryId = cat1,
                    ImageUrl = "/images/products/cement.jpg"
                },
                new Product
                {
                    Name = "طوب أحمر 25×12×6 سم",
                    Description = "طوب أحمر فخاري للبناء",
                    Price = 1200.00m,
                    Stock = 5000,
                    CategoryId = cat1,
                    ImageUrl = "/images/products/red-brick.jpg"
                },
                new Product
                {
                    Name = "حديد تسليح 16 ملم (طن)",
                    Description = "حديد تسليح عالي الجودة",
                    Price = 28000.00m,
                    Stock = 50,
                    CategoryId = cat1,
                    ImageUrl = "/images/products/steel-rebar.jpg"
                },
                new Product
                {
                    Name = "رمل (متر مكعب)",
                    Description = "رمل نظيف للبناء والمحارة",
                    Price = 350.00m,
                    Stock = 200,
                    CategoryId = cat1,
                    ImageUrl = "/images/products/sand.jpg"
                },
                new Product
                {
                    Name = "زلط (متر مكعب)",
                    Description = "زلط مقاس 1-2 سم",
                    Price = 400.00m,
                    Stock = 150,
                    CategoryId = cat1,
                    ImageUrl = "/images/products/gravel.jpg"
                },

                // الدهانات والديكورات
                new Product
                {
                    Name = "دهان بلاستيك داخلي 20 لتر",
                    Description = "دهان بلاستيك قابل للغسيل - ألوان متعددة",
                    Price = 950.00m,
                    Stock = 300,
                    CategoryId = cat2,
                    ImageUrl = "/images/products/interior-paint.jpg"
                },
                new Product
                {
                    Name = "دهان خارجي مقاوم للعوامل الجوية 20 لتر",
                    Description = "دهان خارجي عالي الجودة",
                    Price = 1200.00m,
                    Stock = 200,
                    CategoryId = cat2,
                    ImageUrl = "/images/products/exterior-paint.jpg"
                },
                new Product
                {
                    Name = "معجون حوائط 25 كجم",
                    Description = "معجون أبيض جاهز للاستخدام",
                    Price = 320.00m,
                    Stock = 400,
                    CategoryId = cat2,
                    ImageUrl = "/images/products/wall-putty.jpg"
                },
                new Product
                {
                    Name = "جبس بورد 120×240 سم",
                    Description = "ألواح جبس بورد سمك 12.5 ملم",
                    Price = 180.00m,
                    Stock = 500,
                    CategoryId = cat2,
                    ImageUrl = "/images/products/gypsum-board.jpg"
                },

                // السباكة والصحي
                new Product
                {
                    Name = "مواسير بي في سي 4 بوصة (3 متر)",
                    Description = "مواسير بي في سي للصرف الصحي",
                    Price = 95.00m,
                    Stock = 600,
                    CategoryId = cat3,
                    ImageUrl = "/images/products/pvc-pipe.jpg"
                },
                new Product
                {
                    Name = "خلاط مغسلة كروم",
                    Description = "خلاط مغسلة استانلس كروم",
                    Price = 450.00m,
                    Stock = 150,
                    CategoryId = cat3,
                    ImageUrl = "/images/products/faucet.jpg"
                },
                new Product
                {
                    Name = "حوض مطبخ استانلس",
                    Description = "حوض مطبخ استانلس ستيل 80×50 سم",
                    Price = 850.00m,
                    Stock = 80,
                    CategoryId = cat3,
                    ImageUrl = "/images/products/sink.jpg"
                },
                new Product
                {
                    Name = "مرحاض إفرنجي",
                    Description = "مرحاض إفرنجي بورسلين",
                    Price = 1200.00m,
                    Stock = 60,
                    CategoryId = cat3,
                    ImageUrl = "/images/products/toilet.jpg"
                },

                // الكهرباء والإنارة
                new Product
                {
                    Name = "كابل كهرباء 2.5 ملم (100 متر)",
                    Description = "كابل نحاس مطابق للمواصفات",
                    Price = 650.00m,
                    Stock = 200,
                    CategoryId = cat4,
                    ImageUrl = "/images/products/electric-cable.jpg"
                },
                new Product
                {
                    Name = "مفتاح كهرباء مزدوج",
                    Description = "مفتاح كهرباء بلاستيك عالي الجودة",
                    Price = 35.00m,
                    Stock = 500,
                    CategoryId = cat4,
                    ImageUrl = "/images/products/light-switch.jpg"
                },
                new Product
                {
                    Name = "لمبة LED 15 وات",
                    Description = "لمبة LED موفرة للطاقة",
                    Price = 45.00m,
                    Stock = 800,
                    CategoryId = cat4,
                    ImageUrl = "/images/products/led-bulb.jpg"
                },
                new Product
                {
                    Name = "لوحة كهرباء 12 طريق",
                    Description = "لوحة كهرباء مع قواطع أوتوماتيكية",
                    Price = 850.00m,
                    Stock = 100,
                    CategoryId = cat4,
                    ImageUrl = "/images/products/electrical-panel.jpg"
                },

                // الأخشاب والأبواب
                new Product
                {
                    Name = "باب خشب زان 90 سم",
                    Description = "باب خشب زان كامل الدهان",
                    Price = 3500.00m,
                    Stock = 40,
                    CategoryId = cat5,
                    ImageUrl = "/images/products/wood-door.jpg"
                },
                new Product
                {
                    Name = "لوح خشب موسكي 2×4 (3 متر)",
                    Description = "ألواح خشب موسكي للنجارة",
                    Price = 180.00m,
                    Stock = 300,
                    CategoryId = cat5,
                    ImageUrl = "/images/products/wood-plank.jpg"
                },
                new Product
                {
                    Name = "باركيه HDF 8 ملم",
                    Description = "باركيه ألماني عالي الجودة - متر مربع",
                    Price = 280.00m,
                    Stock = 500,
                    CategoryId = cat5,
                    ImageUrl = "/images/products/parquet.jpg"
                },

                // السيراميك والبورسلين
                new Product
                {
                    Name = "سيراميك أرضيات 60×60 سم",
                    Description = "سيراميك أرضيات إسباني - متر مربع",
                    Price = 120.00m,
                    Stock = 1000,
                    CategoryId = cat6,
                    ImageUrl = "/images/products/floor-ceramic.jpg"
                },
                new Product
                {
                    Name = "سيراميك حوائط 25×50 سم",
                    Description = "سيراميك حوائط للحمامات - متر مربع",
                    Price = 95.00m,
                    Stock = 800,
                    CategoryId = cat6,
                    ImageUrl = "/images/products/wall-ceramic.jpg"
                },
                new Product
                {
                    Name = "بورسلين 80×80 سم",
                    Description = "بورسلين فاخر - متر مربع",
                    Price = 250.00m,
                    Stock = 600,
                    CategoryId = cat6,
                    ImageUrl = "/images/products/porcelain.jpg"
                },
                new Product
                {
                    Name = "رخام جلالة - متر مربع",
                    Description = "رخام جلالة مصري",
                    Price = 450.00m,
                    Stock = 200,
                    CategoryId = cat6,
                    ImageUrl = "/images/products/marble.jpg"
                },

                // الأدوات والمعدات
                new Product
                {
                    Name = "شنيور كهربائي 13 ملم",
                    Description = "شنيور كهربائي 650 وات",
                    Price = 850.00m,
                    Stock = 50,
                    CategoryId = cat7,
                    ImageUrl = "/images/products/drill.jpg"
                },
                new Product
                {
                    Name = "منشار كهربائي",
                    Description = "منشار كهربائي دائري 1200 وات",
                    Price = 1200.00m,
                    Stock = 30,
                    CategoryId = cat7,
                    ImageUrl = "/images/products/saw.jpg"
                },
                new Product
                {
                    Name = "مطرقة 500 جرام",
                    Description = "مطرقة يد احترافية",
                    Price = 85.00m,
                    Stock = 200,
                    CategoryId = cat7,
                    ImageUrl = "/images/products/hammer.jpg"
                },
                new Product
                {
                    Name = "شريط قياس 5 متر",
                    Description = "شريط قياس معدني",
                    Price = 35.00m,
                    Stock = 300,
                    CategoryId = cat7,
                    ImageUrl = "/images/products/tape-measure.jpg"
                },

                // العزل والمواد الكيماوية
                new Product
                {
                    Name = "عازل حراري فوم - متر مربع",
                    Description = "عازل حراري بوليسترين",
                    Price = 120.00m,
                    Stock = 400,
                    CategoryId = cat8,
                    ImageUrl = "/images/products/thermal-insulation.jpg"
                },
                new Product
                {
                    Name = "عازل مائي بيتوميني 18 لتر",
                    Description = "عازل مائي للأسطح",
                    Price = 450.00m,
                    Stock = 150,
                    CategoryId = cat8,
                    ImageUrl = "/images/products/waterproofing.jpg"
                },
                new Product
                {
                    Name = "مادة لاصقة للسيراميك 25 كجم",
                    Description = "غراء سيراميك عالي الجودة",
                    Price = 180.00m,
                    Stock = 300,
                    CategoryId = cat8,
                    ImageUrl = "/images/products/tile-adhesive.jpg"
                },
                new Product
                {
                    Name = "سيليكون شفاف",
                    Description = "سيليكون للتثبيت والعزل",
                    Price = 25.00m,
                    Stock = 500,
                    CategoryId = cat8,
                    ImageUrl = "/images/products/silicone.jpg"
                }
            };

            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();
        }
    }
}
