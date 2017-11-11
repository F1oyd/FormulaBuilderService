import { Component } from "@angular/core";
import * as io from "socket.io-client";
import {} from "@types/websocket";

@Component({
  selector: "app-root",
  templateUrl: "./app.component.html",
  styles: [`
    .mat-card {
      width: 600px;
      margin: auto;
      display: flex;
      flex-direction: column;
    }
  `]
})
export class AppComponent {

  public server = "ws://localhost:2000";
  public connected = false;
  public disabled = false;
  private socket: WebSocket;

  public request = `
    <request>
      <expression>
        <operation>plus<operation>
        <operand>
          <const>20</const>
        </operand>
        <operand>
            <expression>
                <operation>minus<operation>
                <operand>
                    <const>10</const>
                </operand>
                <operand>
                    <const>5</const>
                </operand>
            </expression>
        </operand>
      <expression>
    </request>
  `;

  public toggleConnect() {
    this.disabled = true;
    if (!this.connected) {
      this.socket = new WebSocket(this.server);
      this.socket.onopen = (ev: Event) => {
        this.connected = true;
        this.disabled = false;
      };
    } else {
      this.socket.close();
      this.socket.onclose = (ev: Event) => {
        this.connected = false;
        this.disabled = false;
      };
    }
  }

  public btnClick() {
    const socket = io(this.server);
    socket.on('message', (data) => {
      console.log(data);    
    });
  }

}
