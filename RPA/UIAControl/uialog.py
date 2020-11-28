# -*- coding: utf-8 -*-
import uiautomation as auto
import xml.dom.minidom
import os

#根据鼠标坐标x y，收集操作控件以及所属软件信息
def uia_info(dir, namePrefix):
	if os.path.exists(dir + 'data') == False:
		os.mkdir(os.getcwd() + dir + '//data')

	#当前鼠标的x y绝对坐标
	x, y = auto.GetCursorPos()
	#当前操作控件
	control = currentControl = auto.ControlFromPoint(x, y)

	docInfo = xml.dom.minidom.Document()
	currentControlNode = docInfo.createElement('Control')
	currentControlNode.setAttribute('Name', currentControl.Name)
	currentControlNode.setAttribute('Type', currentControl.ControlTypeName)
	currentControlNode.setAttribute('ClassName', currentControl.ClassName)
	currentControlNode.setAttribute('AutomationId', currentControl.AutomationId)
	docInfo.appendChild(currentControlNode)
	fpInfo = open(dir + "./data/" + namePrefix + "-Info.xml", 'w')
	docInfo.writexml(fpInfo, encoding="utf-8")
	fpInfo.close()

	docXML = xml.dom.minidom.Document()
	root = docXML.createElement('Controlpath')
	docXML.appendChild(root)
	parents = []
  	#找父节点
	while control:
		parents.insert(0, control)
		control = control.GetParentControl()
	i = 0
	for c in parents:
		node = docXML.createElement('Control')
		node.setAttribute('Depth', str(i))
		node.setAttribute('Name', c.Name)
		node.setAttribute('Type', c.ControlTypeName)
		node.setAttribute('Classname', c.ClassName)
		node.setAttribute('AutomationId', c.AutomationId)
		root.appendChild(node)
		i += 1
	fpXML = open(dir + "./data/" + namePrefix + "-XML.xml", 'w')
	docXML.writexml(fpXML, encoding="utf-8")
	fpXML.close()