#include <iostream>
#include <string>
#include <vector>
#include <iomanip>
#include <limits>
using namespace std;

struct Process {
	string processID;
	int arrivalTime;
	int burstTime;
	int remainingTime;
	int completionTime;
	int turnaroundTime;
	int waitingTime;
	int responseTime;
	bool started;
	Process* next;
};

/*
test case:
Process #: 1, Arrival = 0, Burst = 8
           2,         = 1,       = 4
           3,         = 2,       = 9
           4,         = 3,       = 5
*/      

vector<Process> master;                 
Process* readyQueueHead = nullptr;      
vector<pair<string,int> > gantt;
bool simulated = false;

bool higherPriority(Process* a, Process* b) {
	if (a->remainingTime != b->remainingTime) return a->remainingTime < b->remainingTime;
	if (a->arrivalTime  != b->arrivalTime)  return a->arrivalTime  < b->arrivalTime;
	return a->processID < b->processID;
}

void enqueue(Process* n) {
	n->next = nullptr;
	if (!readyQueueHead || higherPriority(n, readyQueueHead)) { 
		n->next = readyQueueHead; 
		readyQueueHead = n; 
	} else {
		Process* c = readyQueueHead;
		while (c->next && !higherPriority(n, c->next)) c = c->next;
		n->next = c->next; c->next = n;
	}
}

void insertProcess() {
	string id; 
	int arr, burst;
	cout << "Enter Process ID: ";
	cin >> ws; getline(cin, id);
	if (id.empty()) { cout << "Invalid input!\nProcess ID cannot be empty.\n"; return; }
	for (size_t i = 0; i < master.size(); i++)
		if (master[i].processID == id) { 
			cout << "Invalid input!\nDuplicate process ID.\n"; 
			return; 
		}
	cout << "Enter Arrival Time: ";
	if (!(cin >> arr) || arr < 0) { 
		cout << "Invalid input!\nArrival time cannot be negative.\n"; 
		cin.clear(); 
		cin.ignore(numeric_limits<streamsize>::max(), '\n'); 
		return; 
	}
	cout << "Enter Burst Time: ";
	if (!(cin >> burst) || burst <= 0) { 
		cout << "Invalid input!\nBurst time must be greater than 0.\n"; 
		cin.clear(); 
		cin.ignore(numeric_limits<streamsize>::max(), '\n'); 
		return;
	}
	Process p;
	p.processID = id; p.arrivalTime = arr; p.burstTime = burst;
	p.remainingTime = burst; p.completionTime = 0; p.turnaroundTime = 0;
	p.waitingTime = 0; p.responseTime = 0; p.started = false; p.next = nullptr;
	master.push_back(p);
	Process* node = new Process(p);
	enqueue(node);
	simulated = false;
	cout << "Process " << id << " inserted into priority queue.\n";
}

void displayQueue() {
	cout << "========================================\n";
	cout << "READY QUEUE\n";
	cout << "========================================\n";
	if (!readyQueueHead) { cout << "(empty)\n"; }
	else {
		cout << left << setw(12) << "Position" << setw(12) << "Process" << "Remaining Time\n";
		cout << "----------------------------------------\n";
		Process* c = readyQueueHead; int pos = 1;
		while (c) { cout << left << setw(12) << pos++ << setw(12) << c->processID << c->remainingTime << "\n"; c = c->next; }
	}
	cout << "========================================\n";
}

Process* getHighestPriority() { return readyQueueHead; }

void removeProcess() {
	if (readyQueueHead) { Process* t = readyQueueHead; readyQueueHead = readyQueueHead->next; delete t; }
}

void simulateSRTF() {
	if (master.empty()) { cout << "No processes. Add processes first.\n"; return; }
	while (readyQueueHead) removeProcess();
	gantt.clear();
	for (size_t i = 0; i < master.size(); i++) {
		master[i].remainingTime = master[i].burstTime; master[i].started = false;
		master[i].completionTime = 0; master[i].turnaroundTime = 0;
		master[i].waitingTime = 0; master[i].responseTime = 0;
	}
	int t = 0, done = 0, n = (int)master.size(); string last = "";
	while (done < n) {
		cout << "Time = " << t << "\n";
		for (size_t i = 0; i < master.size(); i++)
			if (master[i].arrivalTime == t) {
				cout << "New Process Arrived: " << master[i].processID << "\n";
				enqueue(new Process(master[i]));
			}
		cout << "READY QUEUE\n-------------------------\n";
		Process* c = readyQueueHead;
		if (!c) cout << "(empty)\n";
		while (c) { cout << left << setw(5) << c->processID << "Remaining: " << c->remainingTime << "\n"; c = c->next; }
		cout << "-------------------------\n";
		Process* cur = getHighestPriority();
		if (!cur) { cout << "CPU: IDLE\n\n"; t++; continue; }
		cout << "CPU: " << cur->processID << "\n\n";
		if (last != cur->processID) { gantt.push_back(make_pair(cur->processID, t)); last = cur->processID; }
		if (!cur->started) { cur->started = true; cur->responseTime = t - cur->arrivalTime; }
		cur->remainingTime--;
		t++;
		if (cur->remainingTime == 0) {
			cur->completionTime = t;
			cur->turnaroundTime = cur->completionTime - cur->arrivalTime;
			cur->waitingTime = cur->turnaroundTime - cur->burstTime;
			for (size_t i = 0; i < master.size(); i++)
				if (master[i].processID == cur->processID) {
					master[i].completionTime = cur->completionTime;
					master[i].turnaroundTime = cur->turnaroundTime;
					master[i].waitingTime = cur->waitingTime;
					master[i].responseTime = cur->responseTime;
				}
			removeProcess();
			done++;
		}
	}
	cout << "Gantt Chart:\n";
	for (size_t i = 0; i < gantt.size(); i++) cout << left << setw(6) << gantt[i].second;
	cout << t << "\n";
	for (size_t i = 0; i < gantt.size(); i++) cout << "| " << left << setw(4) << gantt[i].first;
	cout << "|\n";
	simulated = true;
}

void displayResults() {
	if (!simulated) { cout << "Run the simulation first (option 3).\n"; return; }
	cout << "========================================\n            SRTF RESULTS\n========================================\n";
	cout << left << setw(10) << "Process" << setw(9) << "Arrival" << setw(7) << "Burst" << setw(12) << "Completion" << "Turnaround\n";
	cout << "--------------------------------------------------\n";
	double tw = 0, tt = 0, tr = 0;
	for (size_t i = 0; i < master.size(); i++) {
		cout << left << setw(10) << master[i].processID << setw(9) << master[i].arrivalTime << setw(7) << master[i].burstTime
		<< setw(12) << master[i].completionTime << master[i].turnaroundTime << "\n";
		tw += master[i].waitingTime; tt += master[i].turnaroundTime; tr += master[i].responseTime;
	}
	cout << "\n" << left << setw(10) << "Process" << setw(14) << "Waiting Time" << "Response Time\n";
	cout << "----------------------------------------\n";
	for (size_t i = 0; i < master.size(); i++)
		cout << left << setw(10) << master[i].processID << setw(14) << master[i].waitingTime << master[i].responseTime << "\n";
	cout << "----------------------------------------\n";
	cout << fixed << setprecision(2);
	cout << "Average Waiting Time    : " << tw / master.size() << "\n";
	cout << "Average Turnaround Time : " << tt / master.size() << "\n";
	cout << "Average Response Time   : " << tr / master.size() << "\n";
	cout << "========================================\n";
}

int main() {
	int choice;
	do {
		cout << "========================================\n";
		cout <<        "        SRTF CPU SCHEDULING\n";
		cout << "========================================\n";
		cout << "1. Add Process\n2. Display Ready Queue\n3. Run SRTF Simulation\n4. Display Results\n5. Exit\n";
		cout << "========================================\n";
		cout << "Enter choice: ";
		if (!(cin >> choice)) { cin.clear(); cin.ignore(numeric_limits<streamsize>::max(), '\n'); choice = 0; }
		switch (choice) {
			case 1: insertProcess(); break;
			case 2: displayQueue(); break;
			case 3: simulateSRTF(); break;
			case 4: displayResults(); break;
			case 5: cout << "Exiting...\n"; break;
			default: cout << "Invalid choice.\n";
		}
	} while (choice != 5);
	return 0;
}
