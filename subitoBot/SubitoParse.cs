using System;
using System.Net;
using HtmlAgilityPack;

namespace subitoBot
{
	public class subitoParse
	{
		public string subitoLink { get; set; } = @"https://www.subito.it";


        public subitoParse()
		{
		}


		public async Task<List<SubitoItemModel>> getListOfItems(int page,string url = null)
		{
			List<SubitoItemModel> listItems = new List<SubitoItemModel>();

			string html = "";
			SubitoClient client = new SubitoClient();

			while (html == "")
			{
				html = await client.getHtmlofItems(page,url);
			}

			HtmlDocument htmlSnippet = new HtmlDocument();
			htmlSnippet.LoadHtml(html);

			try
			{
				foreach (HtmlNode block in htmlSnippet.DocumentNode.SelectNodes($"//div[@class='items__item item-card item-card--small']"))
				{
					
					var atributes = block.Attributes;

					HtmlDocument itemsHtml = new HtmlDocument();
					itemsHtml.LoadHtml(block.InnerHtml);

					var item = itemsHtml.DocumentNode.SelectSingleNode($"//div[@class='SmallCard-module_container__d5-ZC']");

					var link = itemsHtml.DocumentNode.SelectSingleNode($"//a").Attributes["href"].Value;

					var id = link.Replace(".htm", "").Substring(link.LastIndexOf("-")+1);
					try
					{

						var itemModel = new SubitoItemModel()
					{
						Id = long.Parse(id),
						Title = item.SelectSingleNode($"//div[@class='SmallCard-module_item-key-data__fcbjY']").SelectSingleNode($"//h2").InnerText,
                    //Discription = item.SelectSingleNode($"//meta").Attributes["content"].Value,
						ImgLink = item.SelectSingleNode($"//div[@class='SmallCard-module_picture-group__asLo2']").SelectSingleNode($"//div[@class='SmallCard-module_picture-group__asLo2']")
                                        .SelectSingleNode($"//div[@class='SmallCard-module_item-picture__b7vGi CardImage-module_container__HxAgU CardImage-module_small__1NMzM']")
                                        .SelectSingleNode($"//img").Attributes["src"].Value,

						Price = item.SelectSingleNode($"//div[@class='SmallCard-module_item-key-data__fcbjY']").SelectSingleNode($"//div[@class='index-module_container__zrC59']").SelectSingleNode($"//p").InnerText.Replace("Spedizione disponibile",""),
						Link = link, 

					};

					listItems.Add(itemModel);
					}
					catch (Exception e)
					{
						Console.WriteLine("нет картинки");
						try
						{
                            var itemModel = new SubitoItemModel()
                            {
                                Id = long.Parse(id),
                                Title = item.SelectSingleNode($"//div[@class='SmallCard-module_item-key-data__fcbjY']").SelectSingleNode($"//h2").InnerText,
                                //Discription = item.SelectSingleNode($"//meta").Attributes["content"].Value,
                                //ImgLink = item.SelectSingleNode($"//div[@class='SmallCard-module_picture-group__asLo2']").SelectSingleNode($"//div[@class='SmallCard-module_picture-group__asLo2']")
                                //        .SelectSingleNode($"//div[@class='SmallCard-module_item-picture__b7vGi CardImage-module_container__HxAgU CardImage-module_small__1NMzM']")
                                //        .SelectSingleNode($"//img").Attributes["src"].Value,

                                Price = item.SelectSingleNode($"//div[@class='SmallCard-module_item-key-data__fcbjY']").SelectSingleNode($"//div[@class='index-module_container__zrC59']").SelectSingleNode($"//p").InnerText.Replace("Spedizione disponibile", ""),
                                Link = link,

                            };

                            listItems.Add(itemModel);
                        }
						catch (Exception ex)
						{

							//try
							//{
							//	var itemModel = new SubitoItemModel()
							//	{
							//		Id = long.Parse(atributes["data-item-id"].Value),
							//		Title = item.SelectSingleNode($"//div[@class='iva-item-titleStep-pdebR']").SelectSingleNode($"//a").SelectSingleNode($"//h3").InnerText,
							//		//Discription = item.SelectSingleNode($"//meta").Attributes["content"].Value,
							//		Price =
							//		item.SelectSingleNode($"//span[@class='price-text-_YGDY text-text-LurtD text-size-s-BxGpL']").InnerText.ToLower().Contains('а') ?
							//											"0 Р" : item.SelectSingleNode($"//span[@class='price-text-_YGDY text-text-LurtD text-size-s-BxGpL']").InnerText,
							//		Link = subitoLink + "/" + atributes["data-item-id"].Value, //item.SelectSingleNode($"//div[@class='iva-item-titleStep-pdebR']").SelectSingleNode($"//a").Attributes["href"].Value,
							//																   //City = item.SelectSingleNode($"//div[@class='geo-georeferences-SEtee text-text-LurtD text-size-s-BxGpL']").InnerText,


							//	};
							//	listItems.Add(itemModel);
							//}
							//catch (Exception exp)
							//{
							//	Console.WriteLine("Пустое объявление");
							//}

						}

					}
				}
			}
			catch(Exception e)
			{
				Console.WriteLine("проверка на робота");
				return new List<SubitoItemModel>();
			}

            
			return listItems;
        }
	}
}

