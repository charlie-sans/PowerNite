using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerNite.PowerNite.ASM.Runtime;
public class Runtime {

	public static int RIP = 0; // Instruction Pointer

	// Registers
	public static int RAX = 0;
	public static int RBX = 0;
	public static int RCX = 0;
	public Runtime() {
	}
	public static void Run(string[] prog) {
		while (RIP < prog.Length) {
			string line = prog[RIP];
			if (string.IsNullOrWhiteSpace(line)) {
				RIP++;
				continue; // Skip empty lines
			}
			executeInstruction(line);
			RIP++;
		}
	}
	public static void executeInstruction(string line) {
		// Simulate instruction execution
		//Console.WriteLine($"Executing instruction at RIP: {RIP}, Instruction: {line}");
		var tokens = line.Trim().Split(' ');
		if (tokens.Length == 0) return;
		switch (tokens[0].ToUpper()) {
			case "MOV":
				if (tokens.Length == 3) {
					// MOV RAX 5
					switch (tokens[1].ToUpper()) {
						case "RAX":
							RAX = int.Parse(tokens[2]);
							break;
						case "RBX":
							RBX = int.Parse(tokens[2]);
							break;
						case "RCX":
							RCX = int.Parse(tokens[2]);
							break;
						default:
							Console.WriteLine($"Unknown register: {tokens[1]}");
							break;
					}
				}
				break;
			case "ADD":
				switch (tokens.Length) {
					case 3 when tokens[1].ToUpper() == "RAX":
						RAX += int.Parse(tokens[2]);
						break;
					case 3 when tokens[1].ToUpper() == "RBX":
						RBX += int.Parse(tokens[2]);
						break;
					case 3 when tokens[1].ToUpper() == "RCX":
						RCX += int.Parse(tokens[2]);
						break;
					default:
						Console.WriteLine($"Invalid ADD instruction: {line}");
						break;
				}
				break;
			case "OUT":
				// OUT <num|reg>
				switch (tokens[1].ToUpper()) {
					case "RAX":
						Console.WriteLine($" {RAX}");
						break;
					case "RBX":
						Console.WriteLine($" {RBX}");
						break;
					case "RCX":
						Console.WriteLine($" {RCX}");
						break;
					default:
						if (int.TryParse(tokens[1], out int num)) {
							Console.WriteLine($"Output number: {num}");
						} else {
							Console.WriteLine($"Unknown output: {tokens[1]}");
						}
						break;

				}
				break;
			default:
				Console.WriteLine($"Unknown instruction: {line}");
				break;
		}
	}
	public static void PrintRegisters() {
		Console.WriteLine($"RIP: {RIP}");
		Console.WriteLine($"RAX: {RAX}");
		Console.WriteLine($"RBX: {RBX}");
		Console.WriteLine($"RCX: {RCX}");
	}
	public static void Reset() {
		RIP = 0;
		RAX = 0;
		RBX = 0;
		RCX = 0;
	}

}
