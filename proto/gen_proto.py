#!/usr/bin/env python
# -*- coding: utf-8 -*-
import os, sys, re, time
import subprocess
import platform

workpath = sys.path[0]
serverDir = '../server/src/proto'
clientDir = '../client/Assets/Scripts/Game/Proto'

proto_list = []
session_msg = {}

def id(name):
	n = 0
	for c in 'proto.' + name:
		i = ord(c)
		n += (n << 5) + (i << 7) + i
	n &= 0xFFFF
	return n

def checkProtoFiles():
	for root, _, files in os.walk('./'):
		for f in files:
			if not f.endswith('.proto'):
				continue
			proto_list.append(f)
	
def genGo():
	sysstr = platform.system()
	if sysstr == 'Windows':
		suffix = '.exe'
	else:
		suffix = ''
	source = ' '.join(proto_list)
	cmd = 'protoc --plugin=protoc-gen-gogofaster=protoc-gen-gogofaster{} --gogofaster_out={} {}'.format(suffix, serverDir, source)
	print(cmd)
	subprocess.call(cmd.split(' '))

def genMsgInfo():
	remsg = re.compile(r"(?P<com>(//.*\s)*)message\s+(?P<msg>[a-zA-Z]+)\s*\{")
	msgs = []
	coms = {}
	for name in proto_list:
		with open(name, 'r', encoding='utf-8') as f:
			c = f.read()
			m = remsg.search(c)
			n = len(c)
			while m != None:
				i = m.end(0)
				b = 1
				while i < n:
					if c[i] == '{':
						b += 1
						#print('b=',b)
					elif c[i] == '}':
						b -= 1
						#print('b=',b)
					elif c[i] == 'u':
						s = 'uint32 session'
						if c[i:i+len(s)] == s:
							session_msg[m.group('msg')] = True
					i += 1
					if b == 0:
						break
				if b != 0:
					print('syntax error')
					break

				msg = m.group('msg')
				com = m.group('com')
				#print("com", com, msg)
				msgs.append(msg)
				if com :
					coms[msg] = com#'\n	'.join(com.split('\n'))
				else:
					coms[msg] = ''
				m = remsg.search(c, i)
	return msgs, coms

def genGoMsg(msgs, coms):
	Tem = u'''// Generated by cat/proto/gen_proto.py
// DO NOT EDIT!
// Gen Time: {time}

package proto

import (
	"github.com/davyxu/cellnet"
	"github.com/davyxu/cellnet/codec"
	"reflect"
)

const (
{const}
)

func init() {{
{reg}
}}
'''
	Tem_const = u'''	{com}Key{name} int = {msgid}'''
	Tem_reg = u'''		cellnet.RegisterMessageMeta(&cellnet.MessageMeta{{
			Codec: codec.MustGetCodec("gogopb"),
			Type:  reflect.TypeOf((*{name})(nil)).Elem(),
			ID: {msgid},
		}})'''

	w = '{}/proto.msg.go'.format(serverDir)
	print('gen ' + w)
	with open(w, 'w', encoding='utf-8') as f:
		reg = []
		const = []
		for m in msgs:
			d = {'name':m, 'msgid':id(m), 'com':'\n	'.join(coms[m].strip().split('\n'))}
			if d['com']:
				d['com'] = d['com']+'\n\t'
			reg.append(Tem_reg.format(**d))
			const.append(Tem_const.format(**d))
			# print(d, Tem_const.format(**d))
		c = Tem.format(**{'time':time.strftime("%Y-%m-%d %H:%M:%S", time.localtime()), 
			'reg':'\n'.join(reg), 'const':'\n'.join(const)})
		f.write(c)

def genCsharpMsg(msgs, coms):
	Tem = u'''// Generated by cat/proto/gen_proto.py
// DO NOT EDIT!
// Gen Time: {time}

using AM.Game;

namespace Proto {{

	public enum Keys {{
{const}
	}}

	public static class ProtoMsg {{
		public static void init() {{
{reg}
		}}
	}}
	
	public interface ISession {{
		uint Session {{ get; set; }}
		int Err {{ get; set; }}
	}}

{interface}

}}
'''
	Tem_const = u'''		{com}
		{name} = {msgid},'''
	Tem_reg = u'''			MsgMetaSet.RegMsg({msgid}, typeof({name}), ()=>new {name}());'''
	Tem_interface = u'''	public partial class {name} : ISession {{}}'''
	w = '{}/Proto.msg.cs'.format(clientDir)
	print('gen ' + w)
	with open(w, 'w', encoding='utf-8') as f:
		reg = []
		const = []
		interface = []
		# print(session_msg)
		for m in msgs:
			d = {'name':m, 'msgid':id(m), 'com':'\n		'.join(coms[m].strip().split('\n'))}
			# print(Tem_reg.format(**d))
			reg.append(Tem_reg.format(**d))
			# d['com'] = ','.join(coms[m].strip().split('\n//'))
			const.append(Tem_const.format(**d))
			# print(Tem_const.format(**d))
			if (m in session_msg):
				interface.append(Tem_interface.format(**d))
				# print(interface[len(interface)-1])
		d = {'time':time.strftime("%Y-%m-%d %H:%M:%S", time.localtime()), 
			'reg':'\n'.join(reg), 'const':'\n'.join(const), 'interface':'\n'.join(interface)}
		c = Tem.format(**d)
		f.write(c)

def genCsharp():
	source = ' '.join(proto_list)
	cmd = 'protoc --csharp_out={} {}'.format(clientDir, source)
	print(cmd)
	subprocess.call(cmd.split(' '))

def main():
	if len(sys.argv) != 2:
		print("arg error! uses: ./gen_proto.py client|server|all")
		return
	opt = sys.argv[1]
	print(workpath)
	os.chdir(workpath)
	checkProtoFiles()
	msgs, coms = genMsgInfo()
	if opt == 'client' or opt == 'all':
		if not os.path.exists(clientDir):
			os.makedirs(clientDir)
		genCsharp()
		genCsharpMsg(msgs, coms)
	if opt == 'server' or opt == 'all':
		if not os.path.exists(serverDir):
			os.makedirs(serverDir)
		genGo()
		genGoMsg(msgs, coms)

if __name__ == '__main__':
	try:
		main()
	except:
		import traceback
		traceback.print_exc()
		input('runtime error')
