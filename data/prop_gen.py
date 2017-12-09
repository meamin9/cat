# -*- coding: utf-8 -*-

import os,sys
import xml.etree.ElementTree as ET

conf = {
    'entity/role_prop.xml': {'go': '../server/src/role/role_prop.go'}
}

def parse(file, gout):
    root = ET.parse(file).getroot()
    tnode = root.find('type')
    types = {}
    for i, t in enumerate(list(tnode)):
        if types.has_key(t.tag):
            print '%s has already defined' % t.tag
        types[t.tag] = t.attrib
        t.attrib['id'] = (i+1)*1000
    pnode = root.find('prop')
    prop = {}
    for p in list(pnode):
        if prop.has_key(p.tag):
            print "Error prop already defined ", t.tag
        prop[p.tag] = p.attrib
        t = types.get(p.attrib['type'])
        if t is None:
            print "Error prop type not defined", p.attrib['type']
            continue
        p.attrib['id'] = t['id']
        t['id'] += 1
    for n in list(tnode):
        if n.attrib['id']%1000 == 0:
            continue
        go = n.attrib['go']
        s = go.split('.')
        if len(s) > 1:
            n.attrib['go'] = s[-2] + '.' + s[-1]
            s.pop()
            n.attrib['goimport'] = '/'.join(s)
    # golang
    with open(gout, 'w') as f:
        _, nameext = os.path.split(file)
        name, _ = os.path.splitext(nameext)
        name = name[:-5]
        f.write('package ' + name)
        gs = []
        for n in list(tnode):
            g = n.attrib.get('goimport', None)
            if g is not None:
                gs.append('    "%s"' % g)
        if len(gs) > 0:
            f.write('\n\nimport (\n%s\n)' % '\n'.join(gs))
        f.write('\n\nconst (\n')
        for n in list(pnode):
            p = prop[n.tag]
            f.write('    %s int = %d\n' % (n.tag, p['id']))
        f.write(')\n\n')
        pname = name.capitalize() + 'Prop'
        f.write('type %s struct {\n' % pname)
        for n in list(tnode):
            if n.attrib['id']%1000 == 0:
                continue
            f.write('  prop%s  [%d]%s\n' % (n.tag.capitalize(), n.attrib['id']%1000, n.attrib['go']))
        f.write('}\n\n')
        for n in list(tnode):
            if n.attrib['id']%1000 == 0:
                continue
            a = n.tag.capitalize()
            t = n.attrib['go']
            l = 'prop' + a
            f.write('func (self *%s)Get%s(key int) %s {\n    return self.%s[key %% 1000]\n}\n\n' % (pname, a, t, l))
            f.write('func (self *%s)Set%s(key int, v %s) {\n    self.%s[key %% 1000] = v\n}\n\n' % (pname, a, t, l))
        f.write('func New%s() *%s {\n    return &%s{}\n}\n' % (pname, pname, pname))

def main():
    for k, v in conf.iteritems():
        parse(k, v['go'])

if __name__ == '__main__':
    main()
