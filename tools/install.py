#!/usr/bin/env python
# -*- coding: utf-8 -*-
import os, sys
import subprocess
import platform

wp = sys.path[0]

def main():
    gopath = os.environ.get('GOPATH')
    sysstr = platform.system()
    if sysstr == 'Windows':
        sep = ';'
    else:
        sep = ':'
    os.environ['GOPATH'] = wp + sep + gopath
    cmd = [
        'go', 'install', 'protoc-gen-msg'
    ]
    subprocess.call(cmd)

if __name__ == '__main__':
    try:
        main()
    except:
        import traceback
        traceback.print_exc()
        raw_input("")
