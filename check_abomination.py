import os, re

d = r'E:\UNITY\KatyushaPJ\Assets\Resources\Sprites\Enemy\Abomination'
path = os.path.join(d, 'Abomination-Sheet Separated Tags.png.meta')
with open(path, 'r', encoding='utf-8') as f:
    content = f.read()

names = re.findall(r'name: (\w+)', content)
for n in names:
    print(n)

print('---')
m = re.search(r'filterMode: (\d)', content)
val = m.group(1) if m else 'N/A'
print(f'filterMode: {val}')
