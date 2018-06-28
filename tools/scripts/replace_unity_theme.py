#! python2
# -*- coding: utf-8 -*-

def replace(path, source, target):
	with open(path, 'rb+') as f:
		c = f.read().replace(source, target)
		f.seek(0)
		f.write(c)
		f.truncate()

def replace_unity_theme():
	path = 'Unity.exe'
	source = '84C0750833C04883C4205BC38B0348'.decode('hex')
	target = '84C0740833C04883C4205BC38B0348'.decode('hex')
	replace(path, source, target)

def main():
	replace_unity_theme()

if __name__ == '__main__':
	try:
		main()
	except:
		import traceback
		traceback.print_exc()
		raw_input('runtime error!')
