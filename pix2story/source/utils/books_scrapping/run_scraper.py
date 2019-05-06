import os
import urllib.parse
import scrapy
from scrapy.crawler import CrawlerProcess
from scrapy.utils.project import get_project_settings

books_local_dir = '../../../books/adventure/'
os.makedirs(books_local_dir, exist_ok=True)

process = CrawlerProcess(get_project_settings())
process.crawl('bookspider', domain='scrapinghub.com')
process.start() # the script will block here until the crawling is finished
print('Finished scraping books')
