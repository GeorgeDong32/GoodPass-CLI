/* Display.cpp version 2.7.0     */
#include "Display.h"
#include <Windows.h>

void SetColor(int mixedcolor)//���ÿ���̨�����ɫ
{
	HANDLE hCon = GetStdHandle(STD_OUTPUT_HANDLE); //��ȡ���������
	SetConsoleTextAttribute(hCon, mixedcolor); //�����ı�������ɫ
}

void addempty(int oril, int tarl)//��ʽ���������ӿո�
{
	int con = tarl - oril;
	while (con)
	{
		printf(" ");
		con--;
	}
}

void PrintTitle(void)//��ӡ����ͷ
{
	extern string version;
	string dp = "||  Welcome to GoodPass "; string dpend = "  ||";
	printf("*==============================<*>==============================*\n");
	cout << dp << version;
	addempty(dp.length() + version.length() + 6, 67);
	cout << dpend << endl;
	cout << "||  Copyright (c)  GeorgeDong32(Github). All rights reserved.  ||" << endl;
	cout << "*==============================<*>==============================*" << endl << endl;
}

void PrintTestTitle(void)//��ӡ���Գ���ͷ����ɾ������ģʽ���������ϣ�
{
	cout << "#=================<T>=================#" << endl;
	cout << "#  ��ӭ�㣬����ʦ!   �����ǹ���ģʽ!  #" << endl;
	cout << R"(#  ����<tg>�������ɲ���               #
#  ����<td>���н��ܲ���               #
#  ����<rt>�����ظ�����               #)" << endl;
	cout << "#=================<T>=================#" << endl;
}

void printLine(int len)//��ӡ�ָ���
{
	int con = len + 4;
	printf("*");
	while (con)
	{
		printf("-");
		con--;
	}
	printf("*\n");
}

void printNextO(int mode)//�����һ���˵������ڴ�ɾ����
{
	string opt0 = "|  ��������ֵ������һ��  |"; string opt1 = "|  1����������           |";
	string opt2 = "|  2�������ַ�           |"; string opte = "|  �������뿪            |";
	printLine(20);
	cout << opt0 << endl << opt1 << endl << opt2 << endl << opte << endl;
	printLine(20);
}

void printMenu(int mode)//��ӡ�˵�
{
	string dpt = "��������ֵ�Խ��ж�Ӧ������"; string dp0 = "0/e���뿪";
	string dp1 = "1/a�������˺�"; string dp2 = "2/s����ƽ̨�����˺�"; string dp3 = "3/g����ȡָ���˺���Ϣ";
	string dp4 = "4/c���޸�ָ���˺���Ϣ"; string dp5 = "5/d��ɾ��ָ���˺�"; string dp6 = "6����ȡ�����˺�";
	printLine(27);
	printf("|  ");
	cout << dpt << "   |" << endl;
	cout << "|  " << dp0; addempty(dp0.length(), 27);
	cout << "  |" << endl;
	cout << "|  " << dp1; addempty(dp1.length(), 27);
	cout << "  |" << endl;
	cout << "|  " << dp2; addempty(dp2.length(), 27);
	cout << "  |" << endl;
	cout << "|  " << dp3; addempty(dp3.length(), 27);
	cout << "  |" << endl;
	cout << "|  " << dp4; addempty(dp4.length(), 27);
	cout << "  |" << endl;
	cout << "|  " << dp5; addempty(dp5.length(), 27);
	cout << "  |" << endl;
	cout << "|  " << dp6; addempty(dp6.length(), 27);
	cout << "  |" << endl;
	printLine(27);
}

void Displayinf(string d, int pm, int lm, string color)
{
	int cdir = 15;
	if (color == "yellow")
		cdir = 14;
	else if (color == "red")
		cdir = 12;
	else if (color == "green")
		cdir = 10;
	else
		cdir = 15;
	SetColor(cdir);
	int len = d.length();
	if (pm)
	{
		printLine(len - 6);
		cout << d << endl;
		printLine(len - 6);
	}
	else
	{
		printLine(len);
		cout << "|  " << d << "  |" << endl;
		printLine(len);
	}
	SetColor(15);
}

void printDevloping()//��ӡ��������ʾ����Ч��������ɾ����
{
	cout << "/------------------------------+" << endl;
	cout << "|  :)                          |" << endl;
	cout << "|  �¹������ڿ����У������ڴ�  |" << endl;
	cout << "+------------------------------/" << endl;
}

void printaddMenu()
{
	string dis1 = "|  ��ѡ���������뷽ʽ��  |"; string dis2 = "|  m/1���ֶ�����         |";
	string dis3 = "|  g/2������������       |";
	printLine(20);
	cout << dis1 << endl << dis2 << endl << dis3 << endl;
	printLine(20);
}

void printmLine(int len, int cp)//��ӡ��������
{
	//int con = len + 4;
	printf("+");
	int flag = cp;
	for (int i = 0; i < len + 4; i++)
	{
		if (i == cp && flag)
			printf("+");
		else
			printf("-");
	}
	printf("+\n");
}