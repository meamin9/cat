#!/usr/bin/env python
# -*- coding: utf-8 -*-
import os, sys
import subprocess

workpath = sys.path[0]

def gen_csharp():
    out = '../client/Assets/Scripts/Proto'
    if not os.path.exists(out):
        os.makedirs(out)
    for root, _, files in os.walk('./'):
        for f in files:
            if not f.endswith('.proto'):
                continue
            fpath = os.path.join(root, f)
            cmd = 'protoc --csharp_out={} {}'.format(out, fpath) 
            print(cmd)
            subprocess.call(cmd)

def main():
    os.chdir(workpath)
    gen_csharp()
    input('')

if __name__ == '__main__':
    try:
        main()
    except:
        import traceback
        traceback.print_exc()
        input('runtime error')
