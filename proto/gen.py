#!/usr/bin/env python
# -*- coding: utf-8 -*-
import os, sys
import subprocess
import platform

workpath = sys.path[0]
goprotopath = os.path.join(workpath, '..', 'server', 'src', 'proto')
csharppath = os.path.join(workpath, '..', 'client', 'Assets', 'Scripts', 'proto')

protoc = 'protoc'
protoc_gen_go = 'protoc-gen-go'
protoc_gen_msg = 'protoc-gen-msg'

protoc_gen_csharp = 'protoc-gen-sharpnet'

def proto_cmd():
    global protoc, protoc_gen_go, protoc_gen_msg, protoc_gen_csharp
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
        path = os.path.join(p, 'bin', protoc_gen_csharp)
        if os.path.exists(path):
            gensharp = path
    protoc_gen_go = gengo
    protoc_gen_msg = genmsg
    protoc_gen_csharp = gensharp
    print('protoc = ', protoc)
    print('protoc_gen_go = ', protoc_gen_go)
    print('protoc_gen_msg = ', protoc_gen_msg)
    print('protoc_gen_csharp = ', protoc_gen_csharp)

def gen_go_proto(filename):
    print('file: ', filename)
    name, ext = os.path.splitext(filename)
    pname = name + ext[1:]
    out = os.path.join(goprotopath, pname)
    if not os.path.exists(out):
        os.makedirs(out)
    gocmd = [protoc, '--plugin=protoc-gen-go=' + protoc_gen_go, '--go_out=' + out,
             '--proto_path=' + workpath, filename]
    msgcmd = [protoc, '--plugin=protoc-gen-msg=' + protoc_gen_msg, '--msg_out=msgid.go:' + out,
             '--proto_path=' + workpath, filename]
    subprocess.call(gocmd)
    subprocess.call(msgcmd)
    out = csharppath # os.path.join(csharppath, pname)
    if not os.path.exists(out):
        os.makedirs(out)
    csharpcmd = [protoc, '--plugin=protoc-gen-sharpnet=' + protoc_gen_csharp, '--sharpnet_out=' + out,
                 '--proto_path=' + workpath, filename]
    subprocess.call(csharpcmd)

def gen_go_dir(dirname, dirpath):
    out = goprotopath
    for name in os.listdir(dirpath):
        if name.endswith('.proto'):
            gocmd = [protoc, '--plugin=protoc-gen-go=' + protoc_gen_go, '--go_out=' + out,
                     '--proto_path=' + dirpath, name]
            subprocess.call(gocmd)

def walkdir(path):
    for name in os.listdir(path):
        fullpath = os.path.join(path, name)
        _, ext = os.path.splitext(name)
        if ext == '.proto':
            gen_go_proto(name)
        elif os.path.isdir(fullpath):
            gen_go_dir(name, fullpath)

def main():
    proto_cmd()
    walkdir(workpath)

if __name__ == '__main__':
    main()
