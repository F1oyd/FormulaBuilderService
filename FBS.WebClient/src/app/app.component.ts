import { Component } from '@angular/core';

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

  private socket: WebSocket;

  public server = "ws://localhost:53780";
  public connected = false;
  
  public request = `<request>
  <expression>
    <operation>plus</operation>
    <operand>
      <const>20</const>
    </operand>
    <operand>
        <expression>
            <operation>minus</operation>
            <operand>
                <const>10</const>
            </operand>
            <operand>
                <const>5</const>
            </operand>
        </expression>
    </operand>
  </expression>
</request>`;

  public response: string;

  public toggleConnect() {
    if (!this.connected) {
      this.socket = new WebSocket(this.server);
      this.socket.onopen = (ev: Event) => {
        this.connected = true;
      };
    } else {
      this.socket.close();
      this.socket.onclose = (ev: Event) => {
        this.connected = false;
        this.response = null;
      };
    }
  }

  public btnClick() {
    if (this.connected) {
      this.socket.send(this.request);
      this.socket.onmessage = (ev: MessageEvent) => {
        this.response = ev.data;
      };
    }
  }

}
