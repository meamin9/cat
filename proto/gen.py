#!/usr/bin/env python
# -*- coding: utf-8 -*-
import os, sys
import subprocess
import platform

workpath = sys.path[0]
goprotopath = os.path.join(workpath, '..', 'server', 'src', 'proto')

protoc = 'protoc'
protoc_gen_go = 'protoc-gen-go'
protoc_gen_msg = 'protoc-gen-msg'

def proto_cmd():
    global protoc, protoc_gen_go, protoc_gen_msg
    gopath = os.environ.get('GOPATH')
    print('gopath =', gopath)
    sysstr = platform.system()
    if sysstr == 'Windows':
        protoc += '.exe'
        protoc_gen_go += '.exe'
        protoc_gen_msg += '.exe'
        paths = gopath.split(';')
    else:
        paths = gopath.split(':')
    for p in paths:
        path = os.path.join(p, 'bin', protoc_gen_go)
        if os.path.exists(path):
            gengo = path
        path = os.path.join(p, 'bin', protoc_gen_msg)
        if os.path.exists(path):
            genmsg = path
    protoc_gen_go = gengo
    protoc_gen_msg = genmsg
    print('protoc = ', protoc)
    print('protoc_gen_go = ', protoc_gen_go)
    print('protoc_gen_msg = ', protoc_gen_msg)

def gen_go_proto(filename):
    print('file: ', filename)
    name, ext = os.path.splitext(filename)
    pname = name + ext[1:]
    out = os.path.join(goprotopath, pname)
    if not os.path.exists(out):
        os.makedirs(out)
    gocmd = [protoc, '--plugin=' + protoc_gen_go, '--go_out=' + out,
             '--proto_path=' + workpath, filename]
    msgcmd = [protoc, '--plugin=' + protoc_gen_msg, '--msg_out=msgid.go:' + out,
             '--proto_path=' + workpath, filename]
    subprocess.call(gocmd)
    subprocess.call(msgcmd)

def walkdir(path):
    for name in os.listdir(path):
        _, ext = os.path.splitext(name)
        if ext == '.proto':
            gen_go_proto(name)

def main():
    proto_cmd()
    walkdir(workpath)

if __name__ == '__main__':
    main()
