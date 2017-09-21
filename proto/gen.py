#!/usr/bin/env python
# -*- coding: utf-8 -*-
import os, sys
import subprocess

workpath = sys.path[0]
goprotopath = os.path.join(workpath, '..', 'server', 'src', 'proto')

def gen_go_proto(filename):
    name, ext = os.path.splitext(filename)
    pname = name + ext[1:]
    out = os.path.join(goprotopath, pname)
    if not os.path.exists(out):
        os.makedirs(out)
    gocmd = ['protoc', '--plugin=protoc-gen-go', '--go_out=' + out,
             '--proto_path=' + workpath, filename]
    msgcmd = ['protoc', '--plugin=protoc-gen-msg', '--msg_out=msgid.go:' + out,
             '--proto_path=' + workpath, filename]
    subprocess.Popen(gocmd)
    subprocess.Popen(msgcmd)

def walkdir(path):
    for name in os.listdir(path):
        _, ext = os.path.splitext(name)
        if ext == '.proto':
            gen_go_proto(name)

def main():
    walkdir(workpath)

if __name__ == '__main__':
    main()
