package main

import (
	"fmt"
	"net"
	"bufio"
)

func main() {

	fmt.Println("--------------------------------")
	fmt.Println("----   DualDungeon Server   ----")
	fmt.Println("--------------------------------")
	fmt.Println("")


	// listen on all interfaces
	ln, _ := net.Listen("tcp", ":7755")
	fmt.Println("listening..")

	// accept connection on port
	conn, _ := ln.Accept()
	fmt.Println("accepted!")

	// run loop forever (or until ctrl-c)
	for {
		// will listen for message to process ending in newline (\n)
		reader := bufio.NewReader(conn)
		json, _ := reader.ReadString('\n')
		fmt.Println("in:",json)
		
		
		// TODO : make more efficient
		// read bytes and packet
		/*
		packet, _ := reader.ReadBytes(99)
		fmt.Println(packet)
		x := math.Float32frombits(binary.LittleEndian.Uint32(packet[0:4]))
		y := math.Float32frombits(binary.LittleEndian.Uint32(packet[4:8]))
		z := math.Float32frombits(binary.LittleEndian.Uint32(packet[8:12]))
		fmt.Println(x, y, z)*/
		
		
		conn.Write([]byte("test\n"))
		
	}
}