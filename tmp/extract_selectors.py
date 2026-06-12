import re
import os

def extract_selectors(css_content):
    # This is a simple regex for classes and IDs
    # It might pick up some false positives in media queries or values, but it's a start
    classes = set(re.findall(r'\.([a-zA-Z0-9_-]+)', css_content))
    ids = set(re.findall(r'#([a-zA-Z0-9_-]+)', css_content))
    return list(classes), list(ids)

def check_usage(selectors, search_path):
    unused = []
    for s in selectors:
        # Search for .s or class="s" or class='s' or id="s"
        # Since we use Blazor, we also look for @( ... "s" ... )
        # A simple grep-like search
        pattern = s
        found = False
        
        # We'll use ripgrep via another tool if possible, but for a script:
        # This is slow for a whole project. I'll only use this script for extraction.
        pass
    return unused

with open(r'd:\netcore\Retailio\wwwroot\css\sale.css', 'r', encoding='utf-8') as f:
    content = f.read()

classes, ids = extract_selectors(content)
print("CLASSES:", ",".join(classes))
print("IDS:", ",".join(ids))
