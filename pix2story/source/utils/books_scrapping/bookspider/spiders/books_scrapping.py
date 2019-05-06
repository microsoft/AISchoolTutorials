import scrapy
from scrapy.selector import Selector
from scrapy.http import HtmlResponse
import urllib.parse
from scrapy.http import Request,Response


class BlogSpider(scrapy.Spider):
    name = 'bookspider'
    books_local_dir = '../../../books/gutenberg/'
    start_urls = ["""http://www.gutenberg.org/ebooks/search/?query=s.Adventure+%21bsxAdventure&start_index="""+str(i) for i in range(0,10,20)]
    user_agent = 'Mozilla/5.0 (Macintosh; Intel Mac OS X 10_9_3) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/35.0.1916.47 Safari/537.36'

    def parse(self, response):
        for title in response.xpath('//a[(contains(@href,"ebooks/"))]'):
            url_book = response.urljoin(title.xpath('@href').extract_first())
            print('url book',url_book)
            yield Request(
                url=response.urljoin(url_book),
                headers={'User-Agent': self.user_agent},
                callback=self.download_book)


    def download_book(self,response):
        txt_link = response.xpath('//a[(contains(@href, ".txt"))]')
        print('txtlink',txt_link)
        urltxt=txt_link.xpath('@href').extract_first()
        yield Request(
            url=response.urljoin(urltxt),
            headers={'User-Agent': self.user_agent},
            callback=self.save_txt)


    def save_txt(self, response):
        path = self.books_local_dir + response.url.split('/')[-1]
        print(path)
        with open(path, 'wb') as f:
            f.write(response.body)







