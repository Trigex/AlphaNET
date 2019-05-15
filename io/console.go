package io

import (
	"bufio"
	"fmt"
	"os"
)

type Console struct {
	// The 3 stds (heh) currently are just strings. This may be changed in the future to support streams and shit
	// The Unix™ Update when?
	stdin  string
	stdout string
	stderr string
}

func (c *Console) Print(text string) {
	c.stdout = text
	fmt.Print(text)
}

func (c *Console) PrintLn(text string) {
	c.stdout = text
	fmt.Println(text)
}

func (c *Console) PrintC(r rune) {
	c.stdin = string(r)
	fmt.Print(r)
}

func (c *Console) ReadLine() string {
	scanner := bufio.NewScanner(os.Stdin)
	scanner.Scan()
	return scanner.Text()
}

func CreateConsole() Console {
	return Console{}
}
