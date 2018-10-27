#! python3
# -*- coding: utf-8 -*-
import os, sys

def replace(path, source, target):
	with open(path, 'rb+') as f:
		c = f.read().replace(source, target)
		f.seek(0)
		f.write(c)
		f.truncate()

def replace_unity_theme():
	path = 'Unity.exe'
	source = bytes.fromhex('84C0750833C04883C4205BC38B0348')#.decode('hex')
	target = bytes.fromhex('84C0740833C04883C4205BC38B0348')#.decode('hex')
	replace(path, source, target)

def main():
	replace_unity_theme()

if __name__ == '__main__':
	try:
		os.chdir(sys.path[0])
		main()
		input('finished')
	except:
		import traceback
		traceback.print_exc()
		input('runtime error!')
