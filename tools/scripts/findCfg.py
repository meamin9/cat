#! python2
# -*- coding: utf-8 -*-
import os, sys
import xlrd
import time

xlpath = u'G:/3d/Docs/tables/数据配置-主干'
expsheet = u'导出选项'
failedList = []
check_sheet_onlny = False
excel_map = {'_init': False}

def enter_xlpath():
	global xlpath
	if xlpath is None:
		xlpath = sys.path[0]
	os.chdir(xlpath)

def check_excel(xlfile, csvname):
	# print 'find in ', xlfile
	try:
		workbook = xlrd.open_workbook(xlfile)
	except:
		print ''
		import traceback
		t, v, _ = sys.exc_info()
		traceback.print_exception(t, v, None)
		print 'open failed', xlfile
		failedList.append(xlfile)
		# raw_input()
		return False
	with workbook as book:
		sheets = book.sheet_names()
		if check_sheet_onlny:
			for name in sheets:
				excel_map[name.lower()] = xlfile
				if name.lower() == csvname:
					return True
			return False
		if expsheet in sheets:
			export = book.sheet_by_name(expsheet)
			csvs = export.col_values(1, 1)
			# print csvs
			for name in csvs:
				vtype = type(name)
				if vtype != unicode:
					if vtype == float:
						name = "%.0f" % name
					else:
						print '\n未处理的类型', vtype, name, xlfile
						name = str(name)
						raw_input('continue')
				if len(name) == 0:
					break
				excel_map[name.lower()] = xlfile
				if name.lower() == csvname:
					return True
	return False

def find_execel(csvname):
	fpath = excel_map.get(csvname)
	if fpath is not None:
		return fpath
	if not excel_map['_init']:
		for parent, _, files in os.walk(xlpath):
			for f in files:
				name, ext = os.path.splitext(f)
				if ext not in ('.xlsm', '.xlsx', '.xls'):
					continue
				if name.startswith('~$'):
					continue
				print '.',
				fpath = os.path.join(parent, f)
				if check_excel(fpath, csvname):
					return fpath
		excel_map['_init'] = True

def main():
	global failedList
	enter_xlpath()
	while True:
		print u'输入要查找的前端表名字'
		name = raw_input('enter csv file NAME:').lower()
		if name.endswith('.csv'):
			name, _ = os.path.splitext(name)
		print 'find ', name, 'in', xlpath
		begintime = time.time()
		path = find_execel(name)
		elapsed = time.time() - begintime
		print ''
		print '*' * 64
		if len(failedList) != 0:
			print 'Open Failed File List: ', len(failedList)
			for f in failedList:
				print f
			print ''
			failedList = []
		if path is None:
			print '404 meiyou Found', name
		else:
			print 'Found', name, 'IN:\n', path
		print '\ncost %f secs' % elapsed 
		print '*' * 64

if __name__ == '__main__':
	try:
		main()
	except:
		import traceback
		traceback.print_exc()
		raw_input('runtime error!')
