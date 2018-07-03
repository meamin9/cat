#!/usr/bin/env python
# -*- coding: utf-8 -*-
import os, sys
import subprocess
import platform

workpath = sys.path[0]

def gen_go():
    out = '../server/src/proto'
    if not os.path.exists(out):
        os.makedirs(out)
    sysstr = platform.system()
    if sysstr == 'Windows':
        suffix = '.exe'
    else:
        suffix = ''
    for root, _, files in os.walk('./'):
        for f in files:
            if not f.endswith('.proto'):
                continue
            fpath = os.path.join(root, f)
            cmd = 'protoc --plugin=protoc-gen-gogofaster=protoc-gen-gogofaster{} --gogofaster_out={} {}'.format(suffix, out, fpath)
            print(cmd)
            subprocess.call(cmd.split(' '))
            cmd = 'protoc --plugin=protoc-gen-msg=protoc-gen-msg{} --msg_out={}/{}.msg.go:. {}'.format(suffix, out, os.path.splitext(f)[0], fpath)
            print(cmd)
            subprocess.call(cmd.split(' '))

def main():
    print(workpath)
    os.chdir(workpath)
    gen_go()
    #input('press anykey to exit')

if __name__ == '__main__':
    try:
        main()
    except:
        import traceback
        traceback.print_exc()
        input('runtime error')
