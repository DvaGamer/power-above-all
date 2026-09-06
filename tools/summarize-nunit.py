"""Read NUnit failures without dumping full campaign archives."""
import sys
import xml.etree.ElementTree as ET
from collections import Counter
root = ET.parse(sys.argv[1]).getroot()
failures = [case for case in root.iter('test-case') if case.get('result') == 'Failed']
print(root.attrib)
print(Counter(case.get('classname') for case in failures))
for case in failures:
    message = case.findtext('failure/message', '')
    print(case.get('fullname'), message[:160].replace('\n', ' '))
